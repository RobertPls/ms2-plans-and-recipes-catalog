using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Repository.Alimento;
using Catalog.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.EntityFramework.Repository.Alimentos
{
    internal class AlimentoRepository : IAlimentoRepository
    {
        private readonly WriteDbContext _context;

        public AlimentoRepository(WriteDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Alimento alimento)
        {
            await _context.Alimento.AddAsync(alimento);
        }

        public async Task<Alimento?> FindByIdAsync(Guid id)
        {
            return await _context.Alimento.SingleOrDefaultAsync(x => x.Id == id);
        }

        public Task UpdateAsync(Alimento alimento)
        {
            _context.Alimento.Update(alimento);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Alimento alimento)
        {
            _context.Alimento.Remove(alimento);
            return Task.CompletedTask;
        }
    }
}
