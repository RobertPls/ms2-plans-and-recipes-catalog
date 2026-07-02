using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.Receta.AgregarIngrediente
{
    public class AgregarIngredienteCommand : IRequest<bool>
    {
        public Guid RecetaId { get; set; }
        public Guid AlimentoId { get; set; }
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; } = null!;
    }
}
