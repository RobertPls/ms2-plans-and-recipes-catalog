using Catalog.Domain.ValueObjects;
using Shared.Core;

namespace Catalog.Domain.Repository.PlanAlimentario
{
    using PlanAlimentario = Catalog.Domain.Model.PlanesAlimentarios.PlanAlimentario;

    public interface IPlanAlimentarioRepository : IRepository<PlanAlimentario, PlanId>
    {
        Task UpdateAsync(PlanAlimentario plan);
        Task RemoveAsync(PlanAlimentario plan);
    }
}
