using Catalog.Application.Utils;
using MediatR;

namespace Catalog.Application.UseCase.Command.Receta.RemoverIngrediente
{
    public class RemoverIngredienteCommand : IRequest<Result>
    {
        public Guid RecetaId { get; set; }
        public Guid AlimentoId { get; set; }
    }
}
