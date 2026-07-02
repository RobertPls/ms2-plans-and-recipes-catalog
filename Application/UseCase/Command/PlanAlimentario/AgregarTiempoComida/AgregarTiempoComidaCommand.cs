using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AgregarTiempoComida
{
    public class AgregarTiempoComidaCommand : IRequest<bool>
    {
        public Guid PlanId { get; set; }
        public int NumDia { get; set; }
        public string Nombre { get; set; } = null!;
        public int Orden { get; set; }
    }
}
