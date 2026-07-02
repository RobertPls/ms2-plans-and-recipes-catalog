using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Factory.Receta;
using Catalog.Shared.Core;
using Microsoft.Extensions.DependencyInjection;

namespace Catalog.Application
{
    public static class Extensions
    {
        public static IServiceCollection AddApplication(this IServiceCollection services)
        {
            services.AddTransient<IPlanAlimentarioFactory, PlanAlimentarioFactory>();
            services.AddTransient<IRecetaFactory, RecetaFactory>();
            services.AddTransient<IAlimentoFactory, AlimentoFactory>();

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
