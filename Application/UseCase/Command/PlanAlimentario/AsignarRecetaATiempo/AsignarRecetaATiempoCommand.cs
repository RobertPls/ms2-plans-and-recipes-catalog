using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo
{
    public class AsignarRecetaATiempoCommand : IRequest<bool>
    {
        public Guid PlanId { get; set; }
        public int NumDia { get; set; }
        public Guid TiempoComidaId { get; set; }
        public Guid RecetaId { get; set; }
        public decimal RacionCantidad { get; set; }
    }
}
