using Catalog.Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Model.PlanesAlimentarios
{
    public record AsignacionReceta : ValueObject
    {
        public RecetaId RecetaId { get; }
        public Racion Racion { get; }

        private AsignacionReceta() { RecetaId = default!; Racion = default!; }

        public AsignacionReceta(RecetaId recetaId, Racion racion)
        {
            RecetaId = recetaId;
            Racion = racion;
        }
    }
}
