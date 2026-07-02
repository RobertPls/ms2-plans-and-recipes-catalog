using Catalog.Application;
using Catalog.Shared.Core;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.Repository.Receta;
using Catalog.Infrastructure.EntityFramework;
using Catalog.Infrastructure.EntityFramework.Context;
using Catalog.Infrastructure.EntityFramework.Repository.Alimentos;
using Catalog.Infrastructure.EntityFramework.Repository.PlanesAlimentarios;
using Catalog.Infrastructure.EntityFramework.Repository.Recetas;
using Catalog.Infrastructure.Seed;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Infrastructure
{
    public static class Extensions
    {
        public static IServiceCollection AddInfrastructure(this IServiceCollection services,
            IConfiguration configuration)
        {
            services.AddApplication();

            var connectionString = configuration.GetConnectionString("CatalogDbConnectionString");

            services.AddDbContext<ReadDbContext>(context =>
                context.UseSqlite(connectionString));
            services.AddDbContext<WriteDbContext>(context =>
                context.UseSqlite(connectionString));

            services.AddScoped<IPlanAlimentarioRepository, PlanAlimentarioRepository>();
            services.AddScoped<IRecetaRepository, RecetaRepository>();
            services.AddScoped<IAlimentoRepository, AlimentoRepository>();
            services.AddScoped<IUnitOfWork, UnitOfWork>();
            services.AddScoped<IMediator, Mediator>();

            services.AddScoped<IDbInitializer, DbInitializer>();

            services.Scan(scan => scan
                .FromAssembliesOf(typeof(Extensions))
                .AddClasses(classes => classes.AssignableToAny(
                    typeof(IRequestHandler<,>),
                    typeof(INotificationHandler<>)))
                .AsImplementedInterfaces()
                .WithScopedLifetime());

            return services;
        }
    }
}
