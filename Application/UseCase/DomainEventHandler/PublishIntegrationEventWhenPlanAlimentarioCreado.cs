using Shared.Core;
using MediatR;
using Catalog.Domain.Event.PlanAlimentario;
using Shared.IntegrationEvents;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.DomainEventHandler
{
    public class PublishIntegrationEventWhenPlanAlimentarioCreado
        : INotificationHandler<PlanAlimentarioCreado>
    {
        private readonly ILogger<PublishIntegrationEventWhenPlanAlimentarioCreado> _logger;

        public PublishIntegrationEventWhenPlanAlimentarioCreado(
            ILogger<PublishIntegrationEventWhenPlanAlimentarioCreado> logger)
        {
            _logger = logger;
        }

        public async Task Handle(PlanAlimentarioCreado notification, CancellationToken cancellationToken = default)
        {
            var integrationEvent = new CatalogoV1PlanPublicado
            {
                PlanId = notification.PlanId.Value,
                Nombre = notification.Nombre,
                TipoDuracion = notification.TipoDuracion,
                DiasTotales = notification.DiasTotales,
                CantidadTiemposDeComidaPorDia = 0 
            };

            _logger.LogInformation(
                "Published {EventType} for plan {PlanId}: {Nombre}",
                integrationEvent.EventType, integrationEvent.PlanId, integrationEvent.Nombre);

            await Task.CompletedTask;
        }
    }
}
