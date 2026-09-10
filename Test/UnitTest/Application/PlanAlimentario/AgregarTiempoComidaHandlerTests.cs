using Catalog.Application.UseCase.Command.PlanAlimentario.AgregarTiempoComida;
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
    public class AgregarTiempoComidaHandlerTests
    {
        private readonly Mock<IPlanAlimentarioRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AgregarTiempoComidaHandler>> _loggerMock;
        private readonly AgregarTiempoComidaHandler _handler;

        public AgregarTiempoComidaHandlerTests()
        {
            _repoMock = new Mock<IPlanAlimentarioRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AgregarTiempoComidaHandler>>();
            _handler = new AgregarTiempoComidaHandler(_repoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        private static PlanAlimentario CreatePlan() =>
            new PlanAlimentarioFactory().Create("Plan Saludable", new DuracionPlan(TipoDuracion.QUINCENAL), 3);

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var plan = CreatePlan();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AgregarTiempoComidaCommand { PlanId = plan.Id, NumDia = 1, Tipo = (int)TipoTiempoComida.Desayuno };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Tiempo de comida agregado exitosamente", result.Message);
            var tiempo = Assert.Single(plan.DiasDelPlan.First(d => d.NumeroDia == 1).TiemposDeComida);
            Assert.Equal("Desayuno", tiempo.Nombre);
            _repoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == command.PlanId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_PlanNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PlanAlimentario?)null);
            var command = new AgregarTiempoComidaCommand { PlanId = Guid.NewGuid(), NumDia = 1, Tipo = (int)TipoTiempoComida.Desayuno };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Plan alimentario no encontrado", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public async Task Handle_InvalidTipo_ReturnsFailureAndNoCommit(int tipo)
        {
            // Arrange
            var plan = CreatePlan();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AgregarTiempoComidaCommand { PlanId = plan.Id, NumDia = 1, Tipo = tipo };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Tipo de tiempo de comida inválido", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_DayNotInPlan_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var plan = CreatePlan();
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AgregarTiempoComidaCommand { PlanId = plan.Id, NumDia = 16, Tipo = (int)TipoTiempoComida.Desayuno };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Día 16 no existe en el plan", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_DuplicateMealTime_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var plan = CreatePlan();
            plan.AgregarTiempoDeComidaADia(1, TipoTiempoComida.Desayuno);
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new AgregarTiempoComidaCommand { PlanId = plan.Id, NumDia = 1, Tipo = (int)TipoTiempoComida.Desayuno };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Ya existe un tiempo de comida con orden 1", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}