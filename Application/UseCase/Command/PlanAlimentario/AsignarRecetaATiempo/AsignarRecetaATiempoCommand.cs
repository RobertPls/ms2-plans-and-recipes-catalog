using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo
{
    public class AsignarRecetaATiempoCommand : IRequest<Result>
    {
        public Guid PlanId { get; set; }
        public int NumDia { get; set; }
        public Guid TiempoComidaId { get; set; }
        public Guid RecetaId { get; set; }
        public decimal RacionCantidad { get; set; }
    }
}
