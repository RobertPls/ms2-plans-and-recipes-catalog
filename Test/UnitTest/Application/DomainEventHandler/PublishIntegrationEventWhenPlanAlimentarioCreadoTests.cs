using Catalog.Application.UseCase.DomainEventHandler;
using Catalog.Domain.Event.PlanAlimentario;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.Tests.Application
{
    public class PublishIntegrationEventWhenPlanAlimentarioCreadoTests
    {
        private readonly Mock<ILogger<PublishIntegrationEventWhenPlanAlimentarioCreado>> _loggerMock;
        private readonly PublishIntegrationEventWhenPlanAlimentarioCreado _handler;

        public PublishIntegrationEventWhenPlanAlimentarioCreadoTests()
        {
            _loggerMock = new Mock<ILogger<PublishIntegrationEventWhenPlanAlimentarioCreado>>();
            _handler = new PublishIntegrationEventWhenPlanAlimentarioCreado(_loggerMock.Object);
        }

        [Fact]
        public async Task Handle_PlanAlimentarioCreado_LogsPublishedIntegrationEvent()
        {
            // Arrange
            var planId = Guid.NewGuid();
            var notification = new PlanAlimentarioCreado(planId, "Plan Saludable", "QUINCENAL", 15, 3);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) =>
                        state.ToString()!.Contains("CatalogoV1.PlanPublicado") &&
                        state.ToString()!.Contains(planId.ToString()) &&
                        state.ToString()!.Contains("Plan Saludable")),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}