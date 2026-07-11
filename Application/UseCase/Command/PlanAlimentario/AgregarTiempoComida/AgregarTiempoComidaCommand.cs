using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AgregarTiempoComida
{
    public class AgregarTiempoComidaCommand : IRequest<Result>
    {
        public Guid PlanId { get; set; }
        public int NumDia { get; set; }
        public int Tipo { get; set; }
    }
}
