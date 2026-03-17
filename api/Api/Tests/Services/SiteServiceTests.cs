using Api.Data;
using Api.DTOs;
using Api.Infrastructure.Repositories.Interfaces;
using Api.Models;
using Api.Services.Implementations;
using Api.Services.Interfaces;
using Microsoft.EntityFrameworkCore;
using Moq;

namespace Tests.Services
{
    public class SiteServiceTests : IDisposable
    {
        private readonly DataContext _context;
        private readonly Mock<ISiteRepository> _mockSiteRepository;
        private readonly Mock<IMateriauRepository> _mockMateriauRepository;
        private readonly ISiteService _siteService;

        public SiteServiceTests()
        {
            var options = new DbContextOptionsBuilder<DataContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .ConfigureWarnings(w => w.Ignore(Microsoft.EntityFrameworkCore.Diagnostics.InMemoryEventId.TransactionIgnoredWarning))
                .Options;

            _context = new DataContext(options);
            _mockSiteRepository = new Mock<ISiteRepository>();
            _mockMateriauRepository = new Mock<IMateriauRepository>();

            _siteService = new SiteService(
                _context,
                _mockSiteRepository.Object,
                _mockMateriauRepository.Object);
        }

        public void Dispose()
        {
            _context?.Dispose();
        }

        #region GetEnergiesForSiteAsync Tests

        [Fact]
        public async Task GetEnergiesForSiteAsync_ValidSiteWithMultipleEnergies_ReturnsOrderedEnergies()
        {
            // Arrange
            var siteId = 1;
            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            var energies = new List<Energie>
            {
                new Energie 
                { 
                    Id = 1, 
                    SiteId = siteId, 
                    TypeEnergie = "Électricité",
                    ConsommationAnnuelle = 850000,
                    PeriodeDebut = new DateTime(2024, 1, 1),
                    PeriodeFin = new DateTime(2024, 12, 31)
                },
                new Energie 
                { 
                    Id = 2, 
                    SiteId = siteId, 
                    TypeEnergie = "Gaz",
                    ConsommationAnnuelle = 125000,
                    PeriodeDebut = new DateTime(2023, 1, 1),
                    PeriodeFin = new DateTime(2023, 12, 31)
                },
                new Energie 
                { 
                    Id = 3, 
                    SiteId = siteId, 
                    TypeEnergie = "Électricité",
                    ConsommationAnnuelle = 900000,
                    PeriodeDebut = new DateTime(2025, 1, 1),
                    PeriodeFin = new DateTime(2025, 12, 31)
                }
            };

            _context.Energies.AddRange(energies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _siteService.GetEnergiesForSiteAsync(siteId);

            // Assert
            Assert.NotNull(result);
            var energyList = result.ToList();
            Assert.Equal(3, energyList.Count);
            // Should be ordered by PeriodeDebut descending (most recent first)
            Assert.Equal(3, energyList[0].Id); // 2025 first
            Assert.Equal(1, energyList[1].Id); // 2024 second
            Assert.Equal(2, energyList[2].Id); // 2023 last
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task GetEnergiesForSiteAsync_ValidSiteWithNoEnergies_ReturnsEmptyList()
        {
            // Arrange
            var siteId = 1;
            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            // Act
            var result = await _siteService.GetEnergiesForSiteAsync(siteId);

            // Assert
            Assert.NotNull(result);
            Assert.Empty(result);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task GetEnergiesForSiteAsync_NonExistingSite_ThrowsInvalidOperationException()
        {
            // Arrange
            var siteId = 999;
            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _siteService.GetEnergiesForSiteAsync(siteId));
            Assert.Contains("n'existe pas", exception.Message);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task GetEnergiesForSiteAsync_EnergiesWithoutDates_OrderedById()
        {
            // Arrange
            var siteId = 1;
            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            var energies = new List<Energie>
            {
                new Energie 
                { 
                    Id = 1, 
                    SiteId = siteId, 
                    TypeEnergie = "Électricité",
                    ConsommationAnnuelle = 850000
                },
                new Energie 
                { 
                    Id = 2, 
                    SiteId = siteId, 
                    TypeEnergie = "Gaz",
                    ConsommationAnnuelle = 125000
                }
            };

            _context.Energies.AddRange(energies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _siteService.GetEnergiesForSiteAsync(siteId);

            // Assert
            Assert.NotNull(result);
            var energyList = result.ToList();
            Assert.Equal(2, energyList.Count);
            // When no dates, should order by Id descending
            Assert.Equal(2, energyList[0].Id);
            Assert.Equal(1, energyList[1].Id);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task GetEnergiesForSiteAsync_FiltersByCorrectSiteId()
        {
            // Arrange
            var siteId = 1;
            var otherSiteId = 2;
            
            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            var energies = new List<Energie>
            {
                new Energie 
                { 
                    Id = 1, 
                    SiteId = siteId, 
                    TypeEnergie = "Électricité",
                    ConsommationAnnuelle = 850000
                },
                new Energie 
                { 
                    Id = 2, 
                    SiteId = otherSiteId, 
                    TypeEnergie = "Gaz",
                    ConsommationAnnuelle = 125000
                }
            };

            _context.Energies.AddRange(energies);
            await _context.SaveChangesAsync();

            // Act
            var result = await _siteService.GetEnergiesForSiteAsync(siteId);

            // Assert
            Assert.NotNull(result);
            var energyList = result.ToList();
            Assert.Single(energyList);
            Assert.Equal(siteId, energyList[0].Id);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        #endregion

        #region AddEnergieAsync Tests

        [Fact]
        public async Task AddEnergieAsync_ValidRequest_AddsEnergie()
        {
            // Arrange
            var siteId = 1;
            var request = new AddEnergieRequest
            {
                TypeEnergie = "Électricité",
                ConsommationAnnuelle = 1000,
                Unite = "kWh"
            };

            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            // Act
            var result = await _siteService.AddEnergieAsync(siteId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal("Électricité", result.TypeEnergie);
            Assert.Equal(1000, result.ConsommationAnnuelle);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task AddEnergieAsync_NonExistingSite_ThrowsInvalidOperationException()
        {
            // Arrange
            var siteId = 999;
            var request = new AddEnergieRequest { TypeEnergie = "Électricité" };

            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(false);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _siteService.AddEnergieAsync(siteId, request));
            Assert.Contains("n'existe pas", exception.Message);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        [Fact]
        public async Task AddEnergieAsync_InvalidDatePeriod_ThrowsInvalidOperationException()
        {
            // Arrange
            var siteId = 1;
            var request = new AddEnergieRequest
            {
                TypeEnergie = "Électricité",
                PeriodeDebut = DateTime.UtcNow,
                PeriodeFin = DateTime.UtcNow.AddDays(-1)
            };

            _mockSiteRepository.Setup(r => r.ExistsAsync(siteId))
                .ReturnsAsync(true);

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _siteService.AddEnergieAsync(siteId, request));
            Assert.Contains("période de début", exception.Message);
            _mockSiteRepository.Verify(r => r.ExistsAsync(siteId), Times.Once);
            _mockSiteRepository.VerifyAll();
        }

        #endregion

        #region UpdateEnergieAsync Tests

        [Fact]
        public async Task UpdateEnergieAsync_ValidRequest_UpdatesEnergie()
        {
            // Arrange
            var siteId = 1;
            var energieId = 1;
            var energie = new Energie
            {
                Id = energieId,
                SiteId = siteId,
                TypeEnergie = "Électricité",
                ConsommationAnnuelle = 1000
            };

            _context.Energies.Add(energie);
            await _context.SaveChangesAsync();

            var request = new UpdateEnergieRequest
            {
                ConsommationAnnuelle = 2000,
                TypeDonnee = "Réelle"
            };

            // Act
            var result = await _siteService.UpdateEnergieAsync(siteId, energieId, request);

            // Assert
            Assert.NotNull(result);
            Assert.Equal(2000, result.ConsommationAnnuelle);
            Assert.Equal("Réelle", result.TypeDonnee);
        }

        [Fact]
        public async Task UpdateEnergieAsync_NonExistingEnergie_ThrowsInvalidOperationException()
        {
            // Arrange
            var siteId = 1;
            var energieId = 999;
            var request = new UpdateEnergieRequest { ConsommationAnnuelle = 2000 };

            // Act & Assert
            var exception = await Assert.ThrowsAsync<InvalidOperationException>(
                () => _siteService.UpdateEnergieAsync(siteId, energieId, request));
            Assert.Contains("n'existe pas", exception.Message);
        }

        #endregion

        #region DeleteEnergieAsync Tests

        [Fact]
        public async Task DeleteEnergieAsync_ExistingEnergie_DeletesAndReturnsTrue()
        {
            // Arrange
            var siteId = 1;
            var energieId = 1;
            var energie = new Energie
            {
                Id = energieId,
                SiteId = siteId,
                TypeEnergie = "Électricité"
            };

            _context.Energies.Add(energie);
            await _context.SaveChangesAsync();

            // Act
            var result = await _siteService.DeleteEnergieAsync(siteId, energieId);

            // Assert
            Assert.True(result);
            Assert.Empty(_context.Energies);
        }

        [Fact]
        public async Task DeleteEnergieAsync_NonExistingEnergie_ReturnsFalse()
        {
            // Arrange
            var siteId = 1;
            var energieId = 999;

            // Act
            var result = await _siteService.DeleteEnergieAsync(siteId, energieId);

            // Assert
            Assert.False(result);
        }

        #endregion
    }
}
