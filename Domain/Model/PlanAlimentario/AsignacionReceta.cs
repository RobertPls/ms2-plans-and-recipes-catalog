using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Model.PlanesAlimentarios
{
    public record AsignacionReceta : ValueObject
    {
        public Guid RecetaId { get; }
        public Racion Racion { get; }

        private AsignacionReceta() { Racion = default!; }

        public AsignacionReceta(Guid recetaId, Racion racion)
        {
            RecetaId = recetaId;
            Racion = racion;
        }
    }
}
