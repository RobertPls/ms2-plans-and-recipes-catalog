using Catalog.Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Model.Recetas
{
    public record IngredienteReceta : ValueObject
    {
        public AlimentoId AlimentoId { get; }
        public Porcion Porcion { get; }

        private IngredienteReceta() { AlimentoId = default!; Porcion = default!; }

        public IngredienteReceta(AlimentoId alimentoId, Porcion porcion)
        {
            AlimentoId = alimentoId;
            Porcion = porcion;
        }
    }
}
