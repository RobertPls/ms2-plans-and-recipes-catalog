using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.Alimento.CrearAlimento
{
    public class CrearAlimentoCommand : IRequest<Guid>
    {
        public string Nombre { get; set; } = null!;
        public string Categoria { get; set; } = null!;
        public decimal Gramos { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Grasas { get; set; }
    }
}
