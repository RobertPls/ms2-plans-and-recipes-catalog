using Catalog.Infrastructure.EntityFramework.ReadModel.Alimento;
using Catalog.Infrastructure.EntityFramework.ReadModel.PlanAlimentario;
using Catalog.Infrastructure.EntityFramework.ReadModel.Receta;
using Catalog.Infrastructure.EntityFramework.Config.ReadConfig.Alimento;
using Catalog.Infrastructure.EntityFramework.Config.ReadConfig.PlanAlimentario;
using Catalog.Infrastructure.EntityFramework.Config.ReadConfig.Receta;
using Microsoft.EntityFrameworkCore;

namespace Catalog.Infrastructure.EntityFramework.Context
{
    public class ReadDbContext : DbContext
    {
        public virtual DbSet<AlimentoReadModel> Alimento { get; set; } = null!;
        public virtual DbSet<RecetaReadModel> Receta { get; set; } = null!;
        public virtual DbSet<PlanAlimentarioReadModel> PlanAlimentario { get; set; } = null!;

        public ReadDbContext(DbContextOptions<ReadDbContext> options) : base(options)
        {
        }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            var planConfig = new PlanAlimentarioReadConfig();
            modelBuilder.ApplyConfiguration<PlanAlimentarioReadModel>(planConfig);
            modelBuilder.ApplyConfiguration<DiaDelPlanReadModel>(planConfig);
            modelBuilder.ApplyConfiguration<TiempoDeComidaReadModel>(planConfig);
            modelBuilder.ApplyConfiguration<RecetaAsignadaReadModel>(planConfig);

            var recetaConfig = new RecetaReadConfig();
            modelBuilder.ApplyConfiguration<RecetaReadModel>(recetaConfig);
            modelBuilder.ApplyConfiguration<IngredienteRecetaReadModel>(recetaConfig);

            var alimentoConfig = new AlimentoReadConfig();
            modelBuilder.ApplyConfiguration<AlimentoReadModel>(alimentoConfig);
        }
    }
}
