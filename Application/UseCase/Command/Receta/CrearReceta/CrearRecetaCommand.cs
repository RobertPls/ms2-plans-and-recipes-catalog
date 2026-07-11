using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.Receta.CrearReceta
{
    public class CrearRecetaCommand : IRequest<Result<Guid>>
    {
        public string Nombre { get; set; } = null!;
        public string Instrucciones { get; set; } = null!;
    }
}
