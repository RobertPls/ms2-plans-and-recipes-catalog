using Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo;
using Catalog.Domain;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class AsignarRecetaATiempoHandlerTests
    {
        private readonly Mock<IPlanAlimentarioRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AsignarRecetaATiempoHandler>> _loggerMock;
        private readonly AsignarRecetaATiempoHandler _handler;

        public AsignarRecetaATiempoHandlerTests()
        {
            _repoMock = new Mock<IPlanAlimentarioRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AsignarRecetaATiempoHandler>>();
            _handler = new AsignarRecetaATiempoHandler(_repoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        private static PlanAlimentario CreatePlanWithMealTime()
        {
            var plan = new PlanAlimentarioFactory().Create("Plan Saludable", new DuracionPlan(TipoDuracion.QUINCENAL), 3);
            plan.AgregarTiempoDeComidaADia(1, TipoTiempoComida.Desayuno);
            return plan;
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var plan = CreatePlanWithMealTime();
            var tiempo = plan.DiasDelPlan.First(d => d.NumeroDia == 1).TiemposDeComida.Single();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var recetaId = Guid.NewGuid();
            var command = new AsignarRecetaATiempoCommand
            {
                PlanId = plan.Id,
                TiempoComidaId = tiempo.Id,
                Recetas = new List<RecetaItem> { new RecetaItem { RecetaId = recetaId, RacionCantidad = 2 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("1 receta(s) asignada(s) exitosamente", result.Message);
            var asignacion = Assert.Single(tiempo.RecetasAsignadas);
            Assert.Equal(recetaId, asignacion.RecetaId);
            _repoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == command.PlanId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_PlanNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PlanAlimentario?)null);
            var command = new AsignarRecetaATiempoCommand
            {
                PlanId = Guid.NewGuid(),
                TiempoComidaId = Guid.NewGuid(),
                Recetas = new List<RecetaItem> { new RecetaItem { RecetaId = Guid.NewGuid(), RacionCantidad = 1 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Plan alimentario no encontrado", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_TimeNotFoundInPlan_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var plan = CreatePlanWithMealTime();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AsignarRecetaATiempoCommand
            {
                PlanId = plan.Id,
                TiempoComidaId = Guid.NewGuid(),
                Recetas = new List<RecetaItem> { new RecetaItem { RecetaId = Guid.NewGuid(), RacionCantidad = 1 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("no existe en el plan", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_NonPositiveRacion_ReturnsFailureAndNoCommit(int racionCantidad)
        {
            // Arrange
            var plan = CreatePlanWithMealTime();
            var tiempo = plan.DiasDelPlan.First(d => d.NumeroDia == 1).TiemposDeComida.Single();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AsignarRecetaATiempoCommand
            {
                PlanId = plan.Id,
                TiempoComidaId = tiempo.Id,
                Recetas = new List<RecetaItem> { new RecetaItem { RecetaId = Guid.NewGuid(), RacionCantidad = racionCantidad } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Racion must be greater than zero", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_DuplicateRecipeAssignment_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var plan = CreatePlanWithMealTime();
            var tiempo = plan.DiasDelPlan.First(d => d.NumeroDia == 1).TiemposDeComida.Single();
            var recetaId = Guid.NewGuid();
            plan.AsignarRecetaATiempo(tiempo.Id, recetaId, new Racion(1));
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AsignarRecetaATiempoCommand
            {
                PlanId = plan.Id,
                TiempoComidaId = tiempo.Id,
                Recetas = new List<RecetaItem> { new RecetaItem { RecetaId = recetaId, RacionCantidad = 1 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("ya está asignada", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}