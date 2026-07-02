using Catalog.Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Receta
{
    public record RecetaCreada : DomainEvent
    {
        public RecetaId RecetaId { get; }
        public string Nombre { get; }

        public RecetaCreada(RecetaId recetaId, string nombre)
            : base(DateTime.UtcNow)
        {
            RecetaId = recetaId;
            Nombre = nombre;
        }
    }
}
