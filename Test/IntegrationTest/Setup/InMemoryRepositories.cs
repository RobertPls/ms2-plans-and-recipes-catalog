using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.Repository.Receta;
using Shared.Core;
using System.Collections.Concurrent;

namespace Catalog.Tests.IntegrationTest.Setup
{
    public class InMemoryUnitOfWork : IUnitOfWork
    {
        public Task Commit() => Task.CompletedTask;
    }

    public class InMemoryAlimentoRepository : IAlimentoRepository
    {
        private readonly ConcurrentDictionary<Guid, Alimento> _items = new();

        public Task<Alimento?> FindByIdAsync(Guid id) =>
            Task.FromResult<Alimento?>(_items.TryGetValue(id, out var item) ? item : null);

        public Task CreateAsync(Alimento obj)
        {
            _items[obj.Id] = obj;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Alimento alimento)
        {
            _items[alimento.Id] = alimento;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Alimento alimento)
        {
            _items.TryRemove(alimento.Id, out _);
            return Task.CompletedTask;
        }
    }

    public class InMemoryRecetaRepository : IRecetaRepository
    {
        private readonly ConcurrentDictionary<Guid, Receta> _items = new();

        public Task<Receta?> FindByIdAsync(Guid id) =>
            Task.FromResult<Receta?>(_items.TryGetValue(id, out var item) ? item : null);

        public Task CreateAsync(Receta obj)
        {
            _items[obj.Id] = obj;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(Receta receta)
        {
            _items[receta.Id] = receta;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(Receta receta)
        {
            _items.TryRemove(receta.Id, out _);
            return Task.CompletedTask;
        }
    }

    public class InMemoryPlanAlimentarioRepository : IPlanAlimentarioRepository
    {
        private readonly ConcurrentDictionary<Guid, PlanAlimentario> _items = new();

        public Task<PlanAlimentario?> FindByIdAsync(Guid id) =>
            Task.FromResult<PlanAlimentario?>(_items.TryGetValue(id, out var item) ? item : null);

        public Task CreateAsync(PlanAlimentario obj)
        {
            _items[obj.Id] = obj;
            return Task.CompletedTask;
        }

        public Task UpdateAsync(PlanAlimentario plan)
        {
            _items[plan.Id] = plan;
            return Task.CompletedTask;
        }

        public Task RemoveAsync(PlanAlimentario plan)
        {
            _items.TryRemove(plan.Id, out _);
            return Task.CompletedTask;
        }
    }
}