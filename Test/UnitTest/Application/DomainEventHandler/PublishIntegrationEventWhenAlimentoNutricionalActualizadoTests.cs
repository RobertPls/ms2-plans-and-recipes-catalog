using Catalog.Application.UseCase.DomainEventHandler;
using Catalog.Domain.Event.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Xunit;

namespace Catalog.Tests.Application
{
    public class PublishIntegrationEventWhenAlimentoNutricionalActualizadoTests
    {
        private readonly Mock<ILogger<PublishIntegrationEventWhenAlimentoNutricionalActualizado>> _loggerMock;
        private readonly PublishIntegrationEventWhenAlimentoNutricionalActualizado _handler;

        public PublishIntegrationEventWhenAlimentoNutricionalActualizadoTests()
        {
            _loggerMock = new Mock<ILogger<PublishIntegrationEventWhenAlimentoNutricionalActualizado>>();
            _handler = new PublishIntegrationEventWhenAlimentoNutricionalActualizado(_loggerMock.Object);
        }

        [Fact]
        public async Task Handle_AlimentoNutricionalActualizado_LogsPublishedIntegrationEvent()
        {
            // Arrange
            var alimentoId = Guid.NewGuid();
            var anterior = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);
            var nueva = new InfoNutricional(100, 60, 0.5m, 16, 0.3m);
            var notification = new AlimentoNutricionalActualizado(alimentoId, anterior, nueva);

            // Act
            await _handler.Handle(notification, CancellationToken.None);

            // Assert
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Information,
                    It.IsAny<EventId>(),
                    It.Is<It.IsAnyType>((state, _) =>
                        state.ToString()!.Contains("CatalogoV1.AlimentoNutricionalActualizado") &&
                        state.ToString()!.Contains(alimentoId.ToString())),
                    It.IsAny<Exception?>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}