using Shared.Core;

namespace Shared.IntegrationEvents
{
    public record CatalogoV1PlanPublicado : IntegrationEvent
    {
        public string EventType => "CatalogoV1.PlanPublicado";
        public Guid PlanId { get; init; }
        public string Nombre { get; init; } = null!;
        public string TipoDuracion { get; init; } = null!;
        public int DiasTotales { get; init; }
        public int CantidadTiemposDeComidaPorDia { get; init; }
    }
}
