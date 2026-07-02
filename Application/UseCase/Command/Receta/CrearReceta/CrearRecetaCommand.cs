using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Command.Receta.CrearReceta
{
    public class CrearRecetaCommand : IRequest<Guid>
    {
        public string Nombre { get; set; } = null!;
        public string Instrucciones { get; set; } = null!;
    }
}
