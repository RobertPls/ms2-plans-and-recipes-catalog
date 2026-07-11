using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Alimento
{
    public record AlimentoNutricionalActualizado : DomainEvent
    {
        public Guid AlimentoId { get; }
        public InfoNutricional InfoNutricionalAnterior { get; }
        public InfoNutricional InfoNutricionalNueva { get; }

        public AlimentoNutricionalActualizado(Guid alimentoId, InfoNutricional infoNutricionalAnterior, InfoNutricional infoNutricionalNueva)
            : base(DateTime.UtcNow)
        {
            AlimentoId = alimentoId;
            InfoNutricionalAnterior = infoNutricionalAnterior;
            InfoNutricionalNueva = infoNutricionalNueva;
        }
    }
}
