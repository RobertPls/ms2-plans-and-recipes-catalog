using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Factory.Receta;
using MediatR;
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

            services.AddMediatR(cfg => cfg.RegisterServicesFromAssembly(typeof(Extensions).Assembly));

            return services;
        }
    }
}
