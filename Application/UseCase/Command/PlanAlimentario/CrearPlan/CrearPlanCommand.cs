using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan
{
    public class CrearPlanCommand : IRequest<Guid>
    {
        public string Nombre { get; set; } = null!;
        public string DuracionTipo { get; set; } = null!;
        public DateTime FechaInicio { get; set; }
    }
}
