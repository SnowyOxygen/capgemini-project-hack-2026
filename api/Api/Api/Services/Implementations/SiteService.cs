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

            return response;
        }
    }
}
