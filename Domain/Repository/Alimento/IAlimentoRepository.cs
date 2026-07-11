using Shared.Core;

namespace Catalog.Domain.Repository.Alimento
{
    using Alimento = Catalog.Domain.Model.Alimentos.Alimento;

    public interface IAlimentoRepository : IRepository<Alimento, Guid>
    {
        Task UpdateAsync(Alimento alimento);
        Task RemoveAsync(Alimento alimento);
    }
}
