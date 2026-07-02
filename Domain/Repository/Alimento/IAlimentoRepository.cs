using Catalog.Domain.ValueObjects;
using Catalog.Shared.Core;

namespace Catalog.Domain.Repository.Alimento
{
    public interface IAlimentoRepository : IRepository<Model.Alimentos.Alimento, AlimentoId>
    {
        Task<IReadOnlyCollection<Model.Alimentos.Alimento>> FindAllAsync();
        Task<IReadOnlyCollection<Model.Alimentos.Alimento>> FindByCategoryAsync(string categoria);
        Task UpdateAsync(Model.Alimentos.Alimento alimento);
        Task RemoveAsync(Model.Alimentos.Alimento alimento);
    }
}
