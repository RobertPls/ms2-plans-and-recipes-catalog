using Catalog.Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.Alimento
{
    public record AlimentoNutricionalActualizado : DomainEvent
    {
        public AlimentoId AlimentoId { get; }
        public InfoNutricional InfoNutricionalAnterior { get; }
        public InfoNutricional InfoNutricionalNueva { get; }

        public AlimentoNutricionalActualizado(AlimentoId alimentoId, InfoNutricional infoNutricionalAnterior, InfoNutricional infoNutricionalNueva)
            : base(DateTime.UtcNow)
        {
            AlimentoId = alimentoId;
            InfoNutricionalAnterior = infoNutricionalAnterior;
            InfoNutricionalNueva = infoNutricionalNueva;
        }
    }
}
