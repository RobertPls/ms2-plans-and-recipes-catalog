using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan
{
    public class CrearPlanCommand : IRequest<Result<Guid>>
    {
        public string Nombre { get; set; } = null!;
        public string DuracionTipo { get; set; } = null!;
        public int ComidasPorDia { get; set; }
    }
}
