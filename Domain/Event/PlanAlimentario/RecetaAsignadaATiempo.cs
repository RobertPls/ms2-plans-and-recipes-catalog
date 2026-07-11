using Shared.Core;

namespace Catalog.Domain.Event.PlanAlimentario
{
    public record RecetaAsignadaATiempo : DomainEvent
    {
        public Guid PlanId { get; }
        public int NumeroDia { get; }
        public Guid TiempoDeComidaId { get; }
        public Guid RecetaId { get; }
        public decimal RacionCantidad { get; }

        public RecetaAsignadaATiempo(Guid planId, int numeroDia, Guid tiempoDeComidaId, Guid recetaId, decimal racionCantidad)
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
