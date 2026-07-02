using Shared.Core;
using MediatR;
using Catalog.Domain.Event.Alimento;
using Shared.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.DomainEventHandler
{
    public class PublishIntegrationEventWhenAlimentoNutricionalActualizado
        : INotificationHandler<AlimentoNutricionalActualizado>
    {
        private readonly ILogger<PublishIntegrationEventWhenAlimentoNutricionalActualizado> _logger;

        public PublishIntegrationEventWhenAlimentoNutricionalActualizado(
            ILogger<PublishIntegrationEventWhenAlimentoNutricionalActualizado> logger)
        {
            _logger = logger;
        }

        public async Task Handle(AlimentoNutricionalActualizado notification, CancellationToken cancellationToken = default)
        {
            var integrationEvent = new CatalogoV1AlimentoNutricionalActualizado
            {
                AlimentoId = notification.AlimentoId.Value,
                CaloriasAnterior = (double)notification.InfoNutricionalAnterior.Calorias,
                ProteinasAnterior = (double)notification.InfoNutricionalAnterior.Proteinas,
                GrasasAnterior = (double)notification.InfoNutricionalAnterior.Grasas,
                CarbohidratosAnterior = (double)notification.InfoNutricionalAnterior.Carbohidratos,
                CaloriasNueva = (double)notification.InfoNutricionalNueva.Calorias,
                ProteinasNueva = (double)notification.InfoNutricionalNueva.Proteinas,
                GrasasNueva = (double)notification.InfoNutricionalNueva.Grasas,
                CarbohidratosNueva = (double)notification.InfoNutricionalNueva.Carbohidratos
            };

            _logger.LogInformation(
                "Published {EventType} for alimento {AlimentoId}",
                integrationEvent.EventType, integrationEvent.AlimentoId);

            await Task.CompletedTask;
        }
    }
}
