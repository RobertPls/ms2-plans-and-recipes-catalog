using Catalog.Domain.ValueObjects;
using Catalog.Shared.Core;

namespace Catalog.Domain.Repository.PlanAlimentario
{
    public interface IPlanAlimentarioRepository : IRepository<Model.PlanesAlimentarios.PlanAlimentario, PlanId>
    {
        Task<IReadOnlyCollection<Model.PlanesAlimentarios.PlanAlimentario>> FindAllAsync();
        Task UpdateAsync(Model.PlanesAlimentarios.PlanAlimentario plan);
        Task RemoveAsync(Model.PlanesAlimentarios.PlanAlimentario plan);
    }
}
