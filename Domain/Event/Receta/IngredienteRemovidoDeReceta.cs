using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Receta
{
    public record IngredienteRemovidoDeReceta : DomainEvent
    {
        public RecetaId RecetaId { get; }
        public AlimentoId AlimentoId { get; }
        public decimal PorcionCantidad { get; }

        public IngredienteRemovidoDeReceta(RecetaId recetaId, AlimentoId alimentoId, decimal porcionCantidad)
            : base(DateTime.UtcNow)
        {
            RecetaId = recetaId;
            AlimentoId = alimentoId;
            PorcionCantidad = porcionCantidad;
        }
    }
}
