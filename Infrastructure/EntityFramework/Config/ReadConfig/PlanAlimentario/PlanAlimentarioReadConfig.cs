using Catalog.Infrastructure.EntityFramework.ReadModel.PlanAlimentario;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntityFramework.Config.ReadConfig.PlanAlimentario
{
    public class PlanAlimentarioReadConfig : IEntityTypeConfiguration<PlanAlimentarioReadModel>,
        IEntityTypeConfiguration<DiaDelPlanReadModel>,
        IEntityTypeConfiguration<TiempoDeComidaReadModel>,
        IEntityTypeConfiguration<RecetaAsignadaReadModel>
    {
        public void Configure(EntityTypeBuilder<PlanAlimentarioReadModel> builder)
        {
            builder.ToTable("PlanAlimentario");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(500);
            builder.Property(x => x.DuracionTipo).HasColumnName("duracion").HasMaxLength(20);

            builder.HasMany(x => x.DiasDelPlan).WithOne(x => x.PlanAlimentario);
        }

        public void Configure(EntityTypeBuilder<DiaDelPlanReadModel> builder)
        {
            builder.ToTable("DiaDelPlan");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.NumeroDia).HasColumnName("numeroDia");

            builder.HasMany(x => x.TiemposDeComida).WithOne(x => x.DiaDelPlan);
        }

        public void Configure(EntityTypeBuilder<TiempoDeComidaReadModel> builder)
        {
            builder.ToTable("TiempoDeComida");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200);
            builder.Property(x => x.Orden).HasColumnName("orden");

            builder.HasMany(x => x.RecetasAsignadas).WithOne(x => x.TiempoDeComida);
        }

        public void Configure(EntityTypeBuilder<RecetaAsignadaReadModel> builder)
        {
            builder.ToTable("AsignacionReceta");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.RecetaId).HasColumnName("recetaId");
            builder.Property(x => x.RacionCantidad).HasColumnName("racionCantidad").HasPrecision(12, 4);
        }
    }
}
