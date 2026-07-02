using Shared.Core;
using Catalog.Infrastructure.EntityFramework.Context;
using MediatR;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure.EntityFramework
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly WriteDbContext _context;
        private readonly IMediator _mediator;

        public UnitOfWork(WriteDbContext context, IMediator mediator)
        {
            _context = context;
            _mediator = mediator;
        }

        public async Task Commit()
        {
            var domainEvents = _context.ChangeTracker.Entries<Entity<object>>()
                .Select(x => x.Entity.DomainEvents)
                .SelectMany(x => x)
                .ToArray();

            foreach (var @event in domainEvents)
            {
                await _mediator.Publish(@event);
            }

            await _context.SaveChangesAsync();
        }
    }
}
