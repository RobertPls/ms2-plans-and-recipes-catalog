using Catalog.Domain.ValueObjects;
using Catalog.Shared.Core;

namespace Catalog.Domain.Repository.Receta
{
    public interface IRecetaRepository : IRepository<Model.Recetas.Receta, RecetaId>
    {
        Task<IReadOnlyCollection<Model.Recetas.Receta>> FindAllAsync();
        Task UpdateAsync(Model.Recetas.Receta receta);
        Task RemoveAsync(Model.Recetas.Receta receta);
    }
}
