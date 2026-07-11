using Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan;
using Shared.Core;
using Catalog.Domain;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
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
            _handler = new CrearPlanHandler(
                _repoMock.Object, _factoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_CallsRepositoryAndUnitOfWork()
        {
            var command = new CrearPlanCommand
            {
                Nombre = "Plan Saludable",
                DuracionTipo = "QUINCENAL",
                ComidasPorDia = 3
            };

            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            var plan = new PlanAlimentarioFactory().Create(command.Nombre, duracion, 3);

            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<DuracionPlan>(), It.IsAny<int>()))
                .Returns(plan!);
            _repoMock.Setup(r => r.CreateAsync(It.IsAny<PlanAlimentario>())).Returns(Task.CompletedTask);

            await _handler.Handle(command, CancellationToken.None);

            _repoMock.Verify(r => r.CreateAsync(It.IsAny<PlanAlimentario>()), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }
    }
}
