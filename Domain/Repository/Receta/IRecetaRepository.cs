using Shared.Core;

namespace Catalog.Domain.Repository.Receta
{
    using Receta = Catalog.Domain.Model.Recetas.Receta;

    public interface IRecetaRepository : IRepository<Receta, Guid>
    {
        Task UpdateAsync(Receta receta);
        Task RemoveAsync(Receta receta);
    }
}
