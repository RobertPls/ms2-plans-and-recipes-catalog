using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Receta
{
    public record IngredienteAgregadoAReceta : DomainEvent
    {
        public RecetaId RecetaId { get; }
        public AlimentoId AlimentoId { get; }
        public decimal PorcionCantidad { get; }
        public string PorcionUnidad { get; }

        public IngredienteAgregadoAReceta(RecetaId recetaId, AlimentoId alimentoId, decimal porcionCantidad, string porcionUnidad)
            : base(DateTime.UtcNow)
        {
            RecetaId = recetaId;
            AlimentoId = alimentoId;
            PorcionCantidad = porcionCantidad;
            PorcionUnidad = porcionUnidad;
        }
    }
}
