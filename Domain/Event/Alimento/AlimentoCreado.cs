using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Alimento
{
    public record AlimentoCreado : DomainEvent
    {
        public Guid AlimentoId { get; }
        public string Nombre { get; }
        public string Categoria { get; }
        public InfoNutricional InfoNutricional { get; }

        public AlimentoCreado(Guid alimentoId, string nombre, string categoria, InfoNutricional infoNutricional)
            : base(DateTime.UtcNow)
        {
            AlimentoId = alimentoId;
            Nombre = nombre;
            Categoria = categoria;
            InfoNutricional = infoNutricional;
        }
    }
}
