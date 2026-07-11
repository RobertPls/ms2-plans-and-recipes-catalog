using Shared.Core;

namespace Catalog.Domain.Event.PlanAlimentario
{
    public record RecetaRemovidaDeTiempo : DomainEvent
    {
        public Guid PlanId { get; }
        public int NumeroDia { get; }
        public Guid TiempoDeComidaId { get; }
        public Guid RecetaId { get; }

        public RecetaRemovidaDeTiempo(Guid planId, int numeroDia, Guid tiempoDeComidaId, Guid recetaId)
            : base(DateTime.UtcNow)
        {
            PlanId = planId;
            NumeroDia = numeroDia;
            TiempoDeComidaId = tiempoDeComidaId;
            RecetaId = recetaId;
        }
    }
}
