using Catalog.Application.Utils;
using MediatR;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.RemoverRecetaDeTiempo
{
    public class RemoverRecetaDeTiempoCommand : IRequest<Result>
    {
        public Guid PlanId { get; set; }
        public Guid TiempoComidaId { get; set; }
        public Guid RecetaId { get; set; }
    }
}
