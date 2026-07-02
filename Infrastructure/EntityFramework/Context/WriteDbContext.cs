using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.Model.Recetas;
using Catalog.Infrastructure.EntityFramework.Config.WriteConfig.Alimentos;
using Catalog.Infrastructure.EntityFramework.Config.WriteConfig.PlanesAlimentarios;
using Catalog.Infrastructure.EntityFramework.Config.WriteConfig.Recetas;
using Microsoft.EntityFrameworkCore;
using Shared.Core;

namespace Catalog.Infrastructure.EntityFramework.Context
{
    public class WriteDbContext : DbContext
    {
        public virtual DbSet<PlanAlimentario> PlanAlimentario { get; set; } = null!;
        public virtual DbSet<Receta> Receta { get; set; } = null!;
        public virtual DbSet<Alimento> Alimento { get; set; } = null!;

        public WriteDbContext(DbContextOptions<WriteDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var planConfig = new PlanAlimentarioWriteConfig();
            modelBuilder.ApplyConfiguration<PlanAlimentario>(planConfig);
            modelBuilder.ApplyConfiguration<DiaDelPlan>(planConfig);
            modelBuilder.ApplyConfiguration<TiempoDeComida>(planConfig);

            var recetaConfig = new RecetaWriteConfig();
            modelBuilder.ApplyConfiguration<Receta>(recetaConfig);

            var alimentoConfig = new AlimentoWriteConfig();
            modelBuilder.ApplyConfiguration<Alimento>(alimentoConfig);

            modelBuilder.Ignore<DomainEvent>();
        }
    }
}
