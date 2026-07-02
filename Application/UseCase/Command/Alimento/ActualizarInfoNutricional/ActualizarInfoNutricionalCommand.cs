using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Command.Alimento.ActualizarInfoNutricional
{
    public class ActualizarInfoNutricionalCommand : IRequest<bool>
    {
        public Guid AlimentoId { get; set; }
        public decimal Gramos { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Grasas { get; set; }
    }
}
