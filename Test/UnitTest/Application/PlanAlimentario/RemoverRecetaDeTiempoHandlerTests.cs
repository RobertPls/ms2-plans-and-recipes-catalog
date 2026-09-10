using Catalog.Application.UseCase.Command.PlanAlimentario.RemoverRecetaDeTiempo;
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
    public class RemoverRecetaDeTiempoHandlerTests
    {
        private readonly Mock<IPlanAlimentarioRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<RemoverRecetaDeTiempoHandler>> _loggerMock;
        private readonly RemoverRecetaDeTiempoHandler _handler;

        public RemoverRecetaDeTiempoHandlerTests()
        {
            _repoMock = new Mock<IPlanAlimentarioRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RemoverRecetaDeTiempoHandler>>();
            _handler = new RemoverRecetaDeTiempoHandler(_repoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var plan = new PlanAlimentarioFactory().Create("Plan Saludable", new DuracionPlan(TipoDuracion.QUINCENAL), 3);
            plan.AgregarTiempoDeComidaADia(1, TipoTiempoComida.Desayuno);
            var tiempo = plan.DiasDelPlan.First(d => d.NumeroDia == 1).TiemposDeComida.Single();
            var recetaId = Guid.NewGuid();
            plan.AsignarRecetaATiempo(tiempo.Id, recetaId, new Racion(1));
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new RemoverRecetaDeTiempoCommand { PlanId = plan.Id, TiempoComidaId = tiempo.Id, RecetaId = recetaId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Receta removida del tiempo de comida exitosamente", result.Message);
            Assert.Empty(tiempo.RecetasAsignadas);
            _repoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == command.PlanId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_PlanNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((PlanAlimentario?)null);
            var command = new RemoverRecetaDeTiempoCommand { PlanId = Guid.NewGuid(), TiempoComidaId = Guid.NewGuid(), RecetaId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Plan alimentario no encontrado", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_TimeNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var plan = new PlanAlimentarioFactory().Create("Plan Saludable", new DuracionPlan(TipoDuracion.QUINCENAL), 3);
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(plan);
            var command = new RemoverRecetaDeTiempoCommand { PlanId = plan.Id, TiempoComidaId = Guid.NewGuid(), RecetaId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("no existe en el plan", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}