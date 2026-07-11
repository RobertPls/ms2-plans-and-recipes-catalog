using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Model.Recetas
{
    public record IngredienteReceta : ValueObject
    {
        public Guid AlimentoId { get; }
        public Porcion Porcion { get; }

        private IngredienteReceta() { Porcion = default!; }

        public IngredienteReceta(Guid alimentoId, Porcion porcion)
        {
            AlimentoId = alimentoId;
            Porcion = porcion;
        }
    }
}
