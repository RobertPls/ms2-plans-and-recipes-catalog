using Shared.Core;

namespace Catalog.Domain.Event.Receta
{
    public record RecetaCreada : DomainEvent
    {
        public Guid RecetaId { get; }
        public string Nombre { get; }

        public RecetaCreada(Guid recetaId, string nombre)
            : base(DateTime.UtcNow)
        {
            RecetaId = recetaId;
            Nombre = nombre;
        }
    }
}
