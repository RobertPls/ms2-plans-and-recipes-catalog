using Shared.Core;

namespace Shared.IntegrationEvents
{
    public record CatalogoV1AlimentoNutricionalActualizado : IntegrationEvent
    {
        public string EventType => "CatalogoV1.AlimentoNutricionalActualizado";
        public Guid AlimentoId { get; init; }
        public double CaloriasAnterior { get; init; }
        public double ProteinasAnterior { get; init; }
        public double GrasasAnterior { get; init; }
        public double CarbohidratosAnterior { get; init; }
        public double CaloriasNueva { get; init; }
        public double ProteinasNueva { get; init; }
        public double GrasasNueva { get; init; }
        public double CarbohidratosNueva { get; init; }
    }
}
