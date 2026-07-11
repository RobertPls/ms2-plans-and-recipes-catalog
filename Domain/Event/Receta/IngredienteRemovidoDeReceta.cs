using Shared.Core;

namespace Catalog.Domain.Event.Receta
{
    public record IngredienteRemovidoDeReceta : DomainEvent
    {
        public Guid RecetaId { get; }
        public Guid AlimentoId { get; }
        public decimal PorcionCantidad { get; }

        public IngredienteRemovidoDeReceta(Guid recetaId, Guid alimentoId, decimal porcionCantidad)
            : base(DateTime.UtcNow)
        {
            RecetaId = recetaId;
            AlimentoId = alimentoId;
            PorcionCantidad = porcionCantidad;
        }
    }
}
