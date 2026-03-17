using Api.Controllers;
using Api.DTOs;
using Api.Services.Interfaces;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Moq;

namespace Tests.Controllers
{
    public class SitesControllerTests
    {
        private readonly Mock<ISiteService> _mockSiteService;
        private readonly SitesController _controller;

        public SitesControllerTests()
        {
            _mockSiteService = new Mock<ISiteService>();
            _controller = new SitesController(_mockSiteService.Object);
        }

        #region GetEnergies Tests

        [Fact]
        public async Task GetEnergies_ValidSiteId_ReturnsOkWithEnergies()
        {
            // Arrange
            var siteId = 1;
            var energies = new List<EnergieResponse>
            {
                new EnergieResponse 
                { 
                    Id = 1, 
                    TypeEnergie = "Électricité", 
                    ConsommationAnnuelle = 850000,
                    Unite = "kWh",
                    TypeDonnee = "Réelle",
                    PeriodeDebut = new DateTime(2024, 1, 1),
                    PeriodeFin = new DateTime(2024, 12, 31)
                },
                new EnergieResponse 
                { 
                    Id = 2, 
                    TypeEnergie = "Gaz", 
                    ConsommationAnnuelle = 125000,
                    Unite = "kWh",
                    TypeDonnee = "Estimée",
                    PeriodeDebut = new DateTime(2024, 1, 1),
                    PeriodeFin = new DateTime(2024, 12, 31)
                }
            };

            _mockSiteService.Setup(s => s.GetEnergiesForSiteAsync(siteId))
                .ReturnsAsync(energies);

            // Act
            var result = await _controller.GetEnergies(siteId);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<EnergieResponse>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EnergieResponse>>(okObjectResult.Value);
            Assert.Equal(2, returnValue.Count());
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.GetEnergiesForSiteAsync(siteId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task GetEnergies_EmptyHistory_ReturnsOkWithEmptyList()
        {
            // Arrange
            var siteId = 1;
            var energies = new List<EnergieResponse>();

            _mockSiteService.Setup(s => s.GetEnergiesForSiteAsync(siteId))
                .ReturnsAsync(energies);

            // Act
            var result = await _controller.GetEnergies(siteId);

            // Assert
            var okResult = Assert.IsType<ActionResult<IEnumerable<EnergieResponse>>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsAssignableFrom<IEnumerable<EnergieResponse>>(okObjectResult.Value);
            Assert.Empty(returnValue);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.GetEnergiesForSiteAsync(siteId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task GetEnergies_NonExistingSite_ReturnsNotFound()
        {
            // Arrange
            var siteId = 999;

            _mockSiteService.Setup(s => s.GetEnergiesForSiteAsync(siteId))
                .ThrowsAsync(new InvalidOperationException("Le site n'existe pas"));

            // Act
            var result = await _controller.GetEnergies(siteId);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<IEnumerable<EnergieResponse>>>(result);
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.GetEnergiesForSiteAsync(siteId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task GetEnergies_ServiceThrowsException_ReturnsBadRequest()
        {
            // Arrange
            var siteId = 1;

            _mockSiteService.Setup(s => s.GetEnergiesForSiteAsync(siteId))
                .ThrowsAsync(new Exception("Database error"));

            // Act
            var result = await _controller.GetEnergies(siteId);

            // Assert
            var badRequestResult = Assert.IsType<ActionResult<IEnumerable<EnergieResponse>>>(result);
            var badRequestObjectResult = Assert.IsType<BadRequestObjectResult>(badRequestResult.Result);
            Assert.Equal(StatusCodes.Status400BadRequest, badRequestObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.GetEnergiesForSiteAsync(siteId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        #endregion

        #region AddEnergie Tests

        [Fact]
        public async Task AddEnergie_ValidRequest_ReturnsCreatedWithEnergie()
        {
            // Arrange
            var siteId = 1;
            var request = new AddEnergieRequest 
            { 
                TypeEnergie = "Électricité",
                ConsommationAnnuelle = 850000,
                Unite = "kWh"
            };
            var response = new EnergieResponse 
            { 
                Id = 1, 
                TypeEnergie = "Électricité",
                ConsommationAnnuelle = 850000,
                Unite = "kWh"
            };

            _mockSiteService.Setup(s => s.AddEnergieAsync(siteId, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.AddEnergie(siteId, request);

            // Assert
            var createdResult = Assert.IsType<ActionResult<EnergieResponse>>(result);
            var createdAtActionResult = Assert.IsType<CreatedAtActionResult>(createdResult.Result);
            var returnValue = Assert.IsType<EnergieResponse>(createdAtActionResult.Value);
            Assert.Equal("Électricité", returnValue.TypeEnergie);
            Assert.Equal(StatusCodes.Status201Created, createdAtActionResult.StatusCode);
            _mockSiteService.Verify(s => s.AddEnergieAsync(siteId, request), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task AddEnergie_NonExistingSite_ReturnsNotFound()
        {
            // Arrange
            var siteId = 999;
            var request = new AddEnergieRequest { TypeEnergie = "Électricité" };

            _mockSiteService.Setup(s => s.AddEnergieAsync(siteId, request))
                .ThrowsAsync(new InvalidOperationException("Le site n'existe pas"));

            // Act
            var result = await _controller.AddEnergie(siteId, request);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<EnergieResponse>>(result);
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.AddEnergieAsync(siteId, request), Times.Once);
            _mockSiteService.VerifyAll();
        }

        #endregion

        #region UpdateEnergie Tests

        [Fact]
        public async Task UpdateEnergie_ValidRequest_ReturnsOkWithUpdatedEnergie()
        {
            // Arrange
            var siteId = 1;
            var energieId = 1;
            var request = new UpdateEnergieRequest { ConsommationAnnuelle = 2000 };
            var response = new EnergieResponse { Id = energieId, ConsommationAnnuelle = 2000 };

            _mockSiteService.Setup(s => s.UpdateEnergieAsync(siteId, energieId, request))
                .ReturnsAsync(response);

            // Act
            var result = await _controller.UpdateEnergie(siteId, energieId, request);

            // Assert
            var okResult = Assert.IsType<ActionResult<EnergieResponse>>(result);
            var okObjectResult = Assert.IsType<OkObjectResult>(okResult.Result);
            var returnValue = Assert.IsType<EnergieResponse>(okObjectResult.Value);
            Assert.Equal(2000, returnValue.ConsommationAnnuelle);
            Assert.Equal(StatusCodes.Status200OK, okObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.UpdateEnergieAsync(siteId, energieId, request), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task UpdateEnergie_NonExistingEnergie_ReturnsNotFound()
        {
            // Arrange
            var siteId = 1;
            var energieId = 999;
            var request = new UpdateEnergieRequest { ConsommationAnnuelle = 2000 };

            _mockSiteService.Setup(s => s.UpdateEnergieAsync(siteId, energieId, request))
                .ThrowsAsync(new InvalidOperationException("L'énergie n'existe pas"));

            // Act
            var result = await _controller.UpdateEnergie(siteId, energieId, request);

            // Assert
            var notFoundResult = Assert.IsType<ActionResult<EnergieResponse>>(result);
            var notFoundObjectResult = Assert.IsType<NotFoundObjectResult>(notFoundResult.Result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundObjectResult.StatusCode);
            _mockSiteService.Verify(s => s.UpdateEnergieAsync(siteId, energieId, request), Times.Once);
            _mockSiteService.VerifyAll();
        }

        #endregion

        #region DeleteEnergie Tests

        [Fact]
        public async Task DeleteEnergie_ExistingEnergie_ReturnsNoContent()
        {
            // Arrange
            var siteId = 1;
            var energieId = 1;

            _mockSiteService.Setup(s => s.DeleteEnergieAsync(siteId, energieId))
                .ReturnsAsync(true);

            // Act
            var result = await _controller.DeleteEnergie(siteId, energieId);

            // Assert
            var noContentResult = Assert.IsType<NoContentResult>(result);
            Assert.Equal(StatusCodes.Status204NoContent, noContentResult.StatusCode);
            _mockSiteService.Verify(s => s.DeleteEnergieAsync(siteId, energieId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        [Fact]
        public async Task DeleteEnergie_NonExistingEnergie_ReturnsNotFound()
        {
            // Arrange
            var siteId = 1;
            var energieId = 999;

            _mockSiteService.Setup(s => s.DeleteEnergieAsync(siteId, energieId))
                .ReturnsAsync(false);

            // Act
            var result = await _controller.DeleteEnergie(siteId, energieId);

            // Assert
            var notFoundResult = Assert.IsType<NotFoundObjectResult>(result);
            Assert.Equal(StatusCodes.Status404NotFound, notFoundResult.StatusCode);
            _mockSiteService.Verify(s => s.DeleteEnergieAsync(siteId, energieId), Times.Once);
            _mockSiteService.VerifyAll();
        }

        #endregion
    }
}
