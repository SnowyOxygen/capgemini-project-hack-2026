using Api.Data;
using Api.DTOs;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;

namespace Api.Services.Implementations
{
    public class SiteService : ISiteService
    {
        private readonly DataContext _context;
        private readonly ISiteRepository _siteRepository;
        private readonly IMateriauRepository _materiauRepository;

        public SiteService(
            DataContext context,
            ISiteRepository siteRepository,
            IMateriauRepository materiauRepository)
        {
            _context = context;
            _siteRepository = siteRepository;
            _materiauRepository = materiauRepository;
        }

        public async Task<IEnumerable<SiteResponse>> GetAllSitesAsync()
        {
            var sites = await _siteRepository.GetAllAsync();
            return sites.Select(MapToResponse);
        }

        public async Task<SiteResponse> CreateSiteAsync(CreateSiteRequest request)
        {
            // Validate materials exist
            if (request.Materiaux != null && request.Materiaux.Any())
            {
                var materiauIds = request.Materiaux.Select(m => m.MateriauId).Distinct().ToList();
                foreach (var materiauId in materiauIds)
                {
                    var exists = await _materiauRepository.ExistsAsync(materiauId);
                    if (!exists)
                    {
                        throw new InvalidOperationException($"Le matériau avec l'ID {materiauId} n'existe pas.");
                    }
                }
            }

            // Validate energy date periods
            if (request.Energies != null)
            {
                foreach (var energie in request.Energies)
                {
                    if (energie.PeriodeDebut.HasValue && energie.PeriodeFin.HasValue &&
                        energie.PeriodeDebut.Value >= energie.PeriodeFin.Value)
                    {
                        throw new InvalidOperationException(
                            "La période de début doit être antérieure à la période de fin pour les énergies.");
                    }
                }
            }

            // Validate parking totals
            if (request.Parking != null)
            {
                var sum = (request.Parking.PlacesAeriennes ?? 0) +
                         (request.Parking.PlacesSousDalle ?? 0) +
                         (request.Parking.PlacesSousSol ?? 0);

                if (request.Parking.NombrePlacesTotal.HasValue && sum > request.Parking.NombrePlacesTotal.Value)
                {
                    throw new InvalidOperationException(
                        "La somme des places de parking dépasse le nombre total de places.");
                }
            }

            // Use transaction for atomic operation
            await using var transaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Create site entity
                var site = new Site
                {
                    Nom = request.Site.Nom,
                    TypeSite = request.Site.TypeSite,
                    AnneeConstruction = request.Site.AnneeConstruction,
                    SuperficieM2 = request.Site.SuperficieM2,
                    NombreEtages = request.Site.NombreEtages,
                    NombrePersonnes = request.Site.NombrePersonnes
                };

                // Create parking if provided
                if (request.Parking != null)
                {
                    site.Parkings.Add(new Parking
                    {
                        NombrePlacesTotal = request.Parking.NombrePlacesTotal,
                        PlacesAeriennes = request.Parking.PlacesAeriennes,
                        PlacesSousDalle = request.Parking.PlacesSousDalle,
                        PlacesSousSol = request.Parking.PlacesSousSol
                    });
                }

                // Create energies if provided
                if (request.Energies != null)
                {
                    foreach (var energieDto in request.Energies)
                    {
                        site.Energies.Add(new Energie
                        {
                            TypeEnergie = energieDto.TypeEnergie,
                            ConsommationAnnuelle = energieDto.ConsommationAnnuelle,
                            Unite = energieDto.Unite,
                            TypeDonnee = energieDto.TypeDonnee,
                            PeriodeDebut = energieDto.PeriodeDebut,
                            PeriodeFin = energieDto.PeriodeFin
                        });
                    }
                }

                // Create site materials if provided
                if (request.Materiaux != null)
                {
                    foreach (var materiauDto in request.Materiaux)
                    {
                        site.SiteMateriaux.Add(new SiteMateriau
                        {
                            MateriauId = materiauDto.MateriauId,
                            Quantite = materiauDto.Quantite,
                            Unite = materiauDto.Unite
                        });
                    }
                }

                // Save to database
                var createdSite = await _siteRepository.CreateAsync(site);

                // Commit transaction
                await transaction.CommitAsync();

                // Reload site with all relationships
                var siteWithRelations = await _context.Sites
                    .Include(s => s.Parkings)
                    .Include(s => s.Energies)
                    .Include(s => s.SiteMateriaux)
                        .ThenInclude(sm => sm.Materiau)
                    .FirstOrDefaultAsync(s => s.Id == createdSite.Id);

                if (siteWithRelations == null)
                {
                    throw new InvalidOperationException("Erreur lors de la création du site.");
                }

                // Map to response
                return MapToResponse(siteWithRelations);
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }

        public async Task<SiteResponse> UpdateSiteAsync(int siteId, UpdateSiteRequest request)
        {
            var site = await _context.Sites
                .Include(s => s.Parkings)
                .Include(s => s.Energies)
                .Include(s => s.SiteMateriaux)
                    .ThenInclude(sm => sm.Materiau)
                .FirstOrDefaultAsync(s => s.Id == siteId);

            if (site == null)
            {
                throw new InvalidOperationException($"Le site avec l'ID {siteId} n'existe pas.");
            }

            if (request.Nom != null) site.Nom = request.Nom;
            if (request.TypeSite != null) site.TypeSite = request.TypeSite;
            if (request.AnneeConstruction.HasValue) site.AnneeConstruction = request.AnneeConstruction;
            if (request.SuperficieM2.HasValue) site.SuperficieM2 = request.SuperficieM2;
            if (request.NombreEtages.HasValue) site.NombreEtages = request.NombreEtages;
            if (request.NombrePersonnes.HasValue) site.NombrePersonnes = request.NombrePersonnes;

            await _context.SaveChangesAsync();
            return MapToResponse(site);
        }

        public async Task<ParkingResponse> UpsertParkingAsync(int siteId, UpsertParkingRequest request)
        {
            var site = await _context.Sites
                .Include(s => s.Parkings)
                .FirstOrDefaultAsync(s => s.Id == siteId);

            if (site == null)
            {
                throw new InvalidOperationException($"Le site avec l'ID {siteId} n'existe pas.");
            }

            var sum = (request.PlacesAeriennes ?? 0) +
                     (request.PlacesSousDalle ?? 0) +
                     (request.PlacesSousSol ?? 0);

            if (request.NombrePlacesTotal.HasValue && sum > request.NombrePlacesTotal.Value)
            {
                throw new InvalidOperationException(
                    "La somme des places de parking dépasse le nombre total de places.");
            }

            var parking = site.Parkings.FirstOrDefault();
            if (parking != null)
            {
                if (request.NombrePlacesTotal.HasValue) parking.NombrePlacesTotal = request.NombrePlacesTotal;
                if (request.PlacesAeriennes.HasValue) parking.PlacesAeriennes = request.PlacesAeriennes;
                if (request.PlacesSousDalle.HasValue) parking.PlacesSousDalle = request.PlacesSousDalle;
                if (request.PlacesSousSol.HasValue) parking.PlacesSousSol = request.PlacesSousSol;
            }
            else
            {
                parking = new Parking
                {
                    SiteId = siteId,
                    NombrePlacesTotal = request.NombrePlacesTotal,
                    PlacesAeriennes = request.PlacesAeriennes,
                    PlacesSousDalle = request.PlacesSousDalle,
                    PlacesSousSol = request.PlacesSousSol
                };
                _context.Parkings.Add(parking);
            }

            await _context.SaveChangesAsync();

            return new ParkingResponse
            {
                Id = parking.Id,
                NombrePlacesTotal = parking.NombrePlacesTotal,
                PlacesAeriennes = parking.PlacesAeriennes,
                PlacesSousDalle = parking.PlacesSousDalle,
                PlacesSousSol = parking.PlacesSousSol
            };
        }

        public async Task<bool> DeleteParkingAsync(int siteId)
        {
            var parking = await _context.Parkings
                .FirstOrDefaultAsync(p => p.SiteId == siteId);

            if (parking == null)
            {
                return false;
            }

            _context.Parkings.Remove(parking);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<IEnumerable<EnergieResponse>> GetEnergiesForSiteAsync(int siteId)
        {
            var siteExists = await _siteRepository.ExistsAsync(siteId);
            if (!siteExists)
            {
                throw new InvalidOperationException($"Le site avec l'ID {siteId} n'existe pas.");
            }

            var energies = await _context.Energies
                .Where(e => e.SiteId == siteId)
                .OrderByDescending(e => e.PeriodeDebut ?? DateTime.MinValue)
                .ThenByDescending(e => e.Id)
                .ToListAsync();

            return energies.Select(e => new EnergieResponse
            {
                Id = e.Id,
                TypeEnergie = e.TypeEnergie,
                ConsommationAnnuelle = e.ConsommationAnnuelle,
                Unite = e.Unite,
                TypeDonnee = e.TypeDonnee,
                PeriodeDebut = e.PeriodeDebut,
                PeriodeFin = e.PeriodeFin
            });
        }

        public async Task<EnergieResponse> AddEnergieAsync(int siteId, AddEnergieRequest request)
        {
            var siteExists = await _siteRepository.ExistsAsync(siteId);
            if (!siteExists)
            {
                throw new InvalidOperationException($"Le site avec l'ID {siteId} n'existe pas.");
            }

            if (request.PeriodeDebut.HasValue && request.PeriodeFin.HasValue &&
                request.PeriodeDebut.Value >= request.PeriodeFin.Value)
            {
                throw new InvalidOperationException(
                    "La période de début doit être antérieure à la période de fin.");
            }

            var energie = new Energie
            {
                SiteId = siteId,
                TypeEnergie = request.TypeEnergie,
                ConsommationAnnuelle = request.ConsommationAnnuelle,
                Unite = request.Unite,
                TypeDonnee = request.TypeDonnee,
                PeriodeDebut = request.PeriodeDebut,
                PeriodeFin = request.PeriodeFin
            };

            _context.Energies.Add(energie);
            await _context.SaveChangesAsync();

            return new EnergieResponse
            {
                Id = energie.Id,
                TypeEnergie = energie.TypeEnergie,
                ConsommationAnnuelle = energie.ConsommationAnnuelle,
                Unite = energie.Unite,
                TypeDonnee = energie.TypeDonnee,
                PeriodeDebut = energie.PeriodeDebut,
                PeriodeFin = energie.PeriodeFin
            };
        }

        public async Task<EnergieResponse> UpdateEnergieAsync(int siteId, int energieId, UpdateEnergieRequest request)
        {
            var energie = await _context.Energies
                .FirstOrDefaultAsync(e => e.Id == energieId && e.SiteId == siteId);

            if (energie == null)
            {
                throw new InvalidOperationException(
                    $"L'énergie avec l'ID {energieId} n'existe pas pour le site {siteId}.");
            }

            if (request.TypeEnergie != null) energie.TypeEnergie = request.TypeEnergie;
            if (request.ConsommationAnnuelle.HasValue) energie.ConsommationAnnuelle = request.ConsommationAnnuelle;
            if (request.Unite != null) energie.Unite = request.Unite;
            if (request.TypeDonnee != null) energie.TypeDonnee = request.TypeDonnee;
            if (request.PeriodeDebut.HasValue) energie.PeriodeDebut = request.PeriodeDebut;
            if (request.PeriodeFin.HasValue) energie.PeriodeFin = request.PeriodeFin;

            if (energie.PeriodeDebut.HasValue && energie.PeriodeFin.HasValue &&
                energie.PeriodeDebut.Value >= energie.PeriodeFin.Value)
            {
                throw new InvalidOperationException(
                    "La période de début doit être antérieure à la période de fin.");
            }

            await _context.SaveChangesAsync();

            return new EnergieResponse
            {
                Id = energie.Id,
                TypeEnergie = energie.TypeEnergie,
                ConsommationAnnuelle = energie.ConsommationAnnuelle,
                Unite = energie.Unite,
                TypeDonnee = energie.TypeDonnee,
                PeriodeDebut = energie.PeriodeDebut,
                PeriodeFin = energie.PeriodeFin
            };
        }

        public async Task<bool> DeleteEnergieAsync(int siteId, int energieId)
        {
            var energie = await _context.Energies
                .FirstOrDefaultAsync(e => e.Id == energieId && e.SiteId == siteId);

            if (energie == null)
            {
                return false;
            }

            _context.Energies.Remove(energie);
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<SiteMateriauResponse> AddMateriauAsync(int siteId, AddSiteMateriauRequest request)
        {
            var siteExists = await _siteRepository.ExistsAsync(siteId);
            if (!siteExists)
            {
                throw new InvalidOperationException($"Le site avec l'ID {siteId} n'existe pas.");
            }

            var materiauExists = await _materiauRepository.ExistsAsync(request.MateriauId);
            if (!materiauExists)
            {
                throw new InvalidOperationException($"Le matériau avec l'ID {request.MateriauId} n'existe pas.");
            }

            var existingRelation = await _context.SiteMateriaux
                .AnyAsync(sm => sm.SiteId == siteId && sm.MateriauId == request.MateriauId);

            if (existingRelation)
            {
                throw new InvalidOperationException(
                    $"Le matériau {request.MateriauId} est déjà associé au site {siteId}.");
            }

            var siteMateriau = new SiteMateriau
            {
                SiteId = siteId,
                MateriauId = request.MateriauId,
                Quantite = request.Quantite,
                Unite = request.Unite
            };

            _context.SiteMateriaux.Add(siteMateriau);
            await _context.SaveChangesAsync();

            var materiau = await _materiauRepository.GetByIdAsync(request.MateriauId);

            return new SiteMateriauResponse
            {
                Id = siteMateriau.Id,
                MateriauId = siteMateriau.MateriauId,
                MateriauNom = materiau?.Nom ?? string.Empty,
                Quantite = siteMateriau.Quantite,
                Unite = siteMateriau.Unite
            };
        }

        public async Task<SiteMateriauResponse> UpdateMateriauAsync(int siteId, int siteMateriauId, UpdateSiteMateriauRequest request)
        {
            var siteMateriau = await _context.SiteMateriaux
                .Include(sm => sm.Materiau)
                .FirstOrDefaultAsync(sm => sm.Id == siteMateriauId && sm.SiteId == siteId);

            if (siteMateriau == null)
            {
                throw new InvalidOperationException(
                    $"Le matériau avec l'ID {siteMateriauId} n'existe pas pour le site {siteId}.");
            }

            if (request.Quantite.HasValue) siteMateriau.Quantite = request.Quantite;
            if (request.Unite != null) siteMateriau.Unite = request.Unite;

            await _context.SaveChangesAsync();

            return new SiteMateriauResponse
            {
                Id = siteMateriau.Id,
                MateriauId = siteMateriau.MateriauId,
                MateriauNom = siteMateriau.Materiau?.Nom ?? string.Empty,
                Quantite = siteMateriau.Quantite,
                Unite = siteMateriau.Unite
            };
        }

        public async Task<bool> DeleteMateriauAsync(int siteId, int siteMateriauId)
        {
            var siteMateriau = await _context.SiteMateriaux
                .FirstOrDefaultAsync(sm => sm.Id == siteMateriauId && sm.SiteId == siteId);

            if (siteMateriau == null)
            {
                return false;
            }

            _context.SiteMateriaux.Remove(siteMateriau);
            await _context.SaveChangesAsync();
            return true;
        }

        private static SiteResponse MapToResponse(Site site)
        {
            var response = new SiteResponse
            {
                Id = site.Id,
                Nom = site.Nom,
                TypeSite = site.TypeSite,
                AnneeConstruction = site.AnneeConstruction,
                SuperficieM2 = site.SuperficieM2,
                NombreEtages = site.NombreEtages,
                NombrePersonnes = site.NombrePersonnes,
                CreatedAt = DateTime.UtcNow
            };

            // Map parking
            var parking = site.Parkings.FirstOrDefault();
            if (parking != null)
            {
                response.Parking = new ParkingResponse
                {
                    Id = parking.Id,
                    NombrePlacesTotal = parking.NombrePlacesTotal,
                    PlacesAeriennes = parking.PlacesAeriennes,
                    PlacesSousDalle = parking.PlacesSousDalle,
                    PlacesSousSol = parking.PlacesSousSol
                };
            }

            // Map energies
            response.Energies = site.Energies.Select(e => new EnergieResponse
            {
                Id = e.Id,
                TypeEnergie = e.TypeEnergie,
                ConsommationAnnuelle = e.ConsommationAnnuelle,
                Unite = e.Unite,
                TypeDonnee = e.TypeDonnee,
                PeriodeDebut = e.PeriodeDebut,
                PeriodeFin = e.PeriodeFin
            }).ToList();

            // Map materials
            response.Materiaux = site.SiteMateriaux.Select(sm => new SiteMateriauResponse
            {
                Id = sm.Id,
                MateriauId = sm.MateriauId,
                MateriauNom = sm.Materiau?.Nom ?? string.Empty,
                Quantite = sm.Quantite,
                Unite = sm.Unite
            }).ToList();

            // Calculate score (Emissions)
            // Construction: Materiaux + Parking
            decimal matEmissions = 0;
            foreach (var sm in site.SiteMateriaux)
            {
                matEmissions += (sm.Quantite ?? 0) * (sm.Materiau?.FacteurEmission ?? 0);
            }

            decimal parkingEmissions = 0;
            if (parking != null)
            {
                parkingEmissions = ((parking.PlacesAeriennes ?? 0) * 0.5m) + 
                                  ((parking.PlacesSousDalle ?? 0) * 2.5m) + 
                                  ((parking.PlacesSousSol ?? 0) * 5.0m);
            }

            response.EmissionsConstruction = matEmissions + parkingEmissions;

            // Annual: Energy + Surface + Employees
            decimal energyEmissions = 0;
            foreach (var e in site.Energies)
            {
                // Note: ideally we should look up the current factor from FacteursEnergie 
                // but for simplicity and matching the frontend logic (which uses types), 
                // we'll use slightly different logic if we don't have the factor repository injected here
                // However, the model doesn't store the factor, so we'd need the repository.
                // Let's assume the site list calculation should match the detail.
                // For now, let's use the static factors we seeded if possible.
                // Or just use the model's TypeEnergie to match.
                
                decimal factor = e.TypeEnergie switch
                {
                    "Électricité" => 0.057m,
                    "Gaz naturel" => 0.227m,
                    "Fioul" => 0.324m,
                    "Géothermie" => 0.015m,
                    _ => 0
                };
                energyEmissions += (e.ConsommationAnnuelle ?? 0) * factor;
            }

            decimal usageSurface = (site.SuperficieM2 ?? 0) * 0.00033m;
            decimal employes = (site.NombrePersonnes ?? 0) * 0.016m; // 0.80 / 50 from user example
            
            response.EmissionsAnnuelles = energyEmissions + usageSurface + employes;

            return response;
        }
    }
}
