using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.PlanAlimentario
{
    public record RecetaAsignadaATiempo : DomainEvent
    {
        public PlanId PlanId { get; }
        public int NumeroDia { get; }
        public Guid TiempoDeComidaId { get; }
        public RecetaId RecetaId { get; }
        public decimal RacionCantidad { get; }

        public RecetaAsignadaATiempo(PlanId planId, int numeroDia, Guid tiempoDeComidaId, RecetaId recetaId, decimal racionCantidad)
            : base(DateTime.UtcNow)
        {
            PlanId = planId;
            NumeroDia = numeroDia;
            TiempoDeComidaId = tiempoDeComidaId;
            RecetaId = recetaId;
            RacionCantidad = racionCantidad;
        }
    }
}
