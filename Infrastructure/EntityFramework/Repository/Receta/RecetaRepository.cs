using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.EntityFramework.Repository.Recetas
{
    internal class RecetaRepository : IRecetaRepository
    {
        private readonly WriteDbContext _context;

        public RecetaRepository(WriteDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(Receta receta)
        {
            await _context.Receta.AddAsync(receta);
        }

        public async Task<Receta?> FindByIdAsync(RecetaId id)
        {
            return await _context.Receta
                .Include("_ingredientes")
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public Task UpdateAsync(Receta receta)
        {
            _context.Receta.Update(receta);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Receta receta)
        {
            _context.Receta.Remove(receta);
            return Task.CompletedTask;
        }
    }
}
