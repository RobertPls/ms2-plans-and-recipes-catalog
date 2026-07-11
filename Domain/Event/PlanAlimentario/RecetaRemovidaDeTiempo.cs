using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.PlanAlimentario
{
    public record RecetaRemovidaDeTiempo : DomainEvent
    {
        public PlanId PlanId { get; }
        public int NumeroDia { get; }
        public Guid TiempoDeComidaId { get; }
        public RecetaId RecetaId { get; }

        public RecetaRemovidaDeTiempo(PlanId planId, int numeroDia, Guid tiempoDeComidaId, RecetaId recetaId)
            : base(DateTime.UtcNow)
        {
            PlanId = planId;
            NumeroDia = numeroDia;
            TiempoDeComidaId = tiempoDeComidaId;
            RecetaId = recetaId;
        }
    }
}
