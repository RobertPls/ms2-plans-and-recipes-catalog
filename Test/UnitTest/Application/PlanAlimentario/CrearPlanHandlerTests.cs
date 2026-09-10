using Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan;
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
    public class CrearPlanHandlerTests
    {
        private readonly Mock<IPlanAlimentarioRepository> _repoMock;
        private readonly Mock<IPlanAlimentarioFactory> _factoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CrearPlanHandler>> _loggerMock;
        private readonly CrearPlanHandler _handler;

        public CrearPlanHandlerTests()
        {
            _repoMock = new Mock<IPlanAlimentarioRepository>();
            _factoryMock = new Mock<IPlanAlimentarioFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CrearPlanHandler>>();
            _handler = new CrearPlanHandler(_repoMock.Object, _factoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var plan = new PlanAlimentarioFactory().Create("Plan Saludable", new DuracionPlan(TipoDuracion.QUINCENAL), 3);
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<DuracionPlan>(), It.IsAny<int>())).Returns(plan);
            var command = new CrearPlanCommand { Nombre = "Plan Saludable", DuracionTipo = "QUINCENAL", ComidasPorDia = 3 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(plan.Id, result.Value);
            Assert.Equal("Plan alimentario creado exitosamente", result.Message);
            _factoryMock.Verify(f => f.Create("Plan Saludable", It.Is<DuracionPlan>(d => d.Tipo == TipoDuracion.QUINCENAL), 3), Times.Once);
            _repoMock.Verify(r => r.CreateAsync(It.Is<PlanAlimentario>(p => p.Nombre.Name == "Plan Saludable")), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Theory]
        [InlineData("SEMANAL")]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_InvalidDurationType_ReturnsFailureAndNoCommit(string? duracionTipo)
        {
            // Arrange
            var command = new CrearPlanCommand { Nombre = "Plan", DuracionTipo = duracionTipo!, ComidasPorDia = 3 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            _factoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<DuracionPlan>(), It.IsAny<int>()), Times.Never);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<PlanAlimentario>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData("")]
        [InlineData(null)]
        public async Task Handle_InvalidName_FactoryThrows_ReturnsFailureAndNoCommit(string? nombre)
        {
            // Arrange
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<DuracionPlan>(), It.IsAny<int>()))
                .Throws(new BussinessRuleValidationException("Plan name cannot be null"));
            var command = new CrearPlanCommand { Nombre = nombre!, DuracionTipo = "QUINCENAL", ComidasPorDia = 3 };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Plan name cannot be null", result.Message);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<PlanAlimentario>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}