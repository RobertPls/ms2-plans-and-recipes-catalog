using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.EntityFramework.Context;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.EntityFramework.Repository.PlanesAlimentarios
{
    internal class PlanAlimentarioRepository : IPlanAlimentarioRepository
    {
        private readonly WriteDbContext _context;

        public PlanAlimentarioRepository(WriteDbContext context)
        {
            _context = context;
        }

        public async Task CreateAsync(PlanAlimentario plan)
        {
            await _context.PlanAlimentario.AddAsync(plan);
        }

        public async Task<PlanAlimentario?> FindByIdAsync(PlanId id)
        {
            return await _context.PlanAlimentario
                .Include("_diasDelPlan._tiemposDeComida._recetasAsignadas")
                .SingleOrDefaultAsync(x => x.Id.Equals(id));
        }

        public Task UpdateAsync(PlanAlimentario plan)
        {
            _context.PlanAlimentario.Update(plan);
            return Task.CompletedTask;
        }

        public Task RemoveAsync(PlanAlimentario plan)
        {
            _context.PlanAlimentario.Remove(plan);
            return Task.CompletedTask;
        }
    }
}
