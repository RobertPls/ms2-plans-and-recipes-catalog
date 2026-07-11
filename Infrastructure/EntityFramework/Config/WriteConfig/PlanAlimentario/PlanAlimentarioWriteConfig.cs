using Catalog.Domain;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.Infrastructure.EntityFramework.Config.WriteConfig.PlanesAlimentarios
{
    public class PlanAlimentarioWriteConfig : IEntityTypeConfiguration<PlanAlimentario>,
        IEntityTypeConfiguration<DiaDelPlan>,
        IEntityTypeConfiguration<TiempoDeComida>
    {
        public void Configure(EntityTypeBuilder<PlanAlimentario> builder)
        {
            builder.ToTable("PlanAlimentario");
            builder.HasKey(x => x.Id);

            var planNameConverter = new ValueConverter<PlanName, string>(
                name => name.Name,
                s => (PlanName)s
            );

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.Nombre)
                .HasConversion(planNameConverter)
                .HasColumnName("nombre")
                .HasMaxLength(500);

            var duracionConverter = new ValueConverter<DuracionPlan, string>(
                d => d.Tipo.ToString(),
                s => new DuracionPlan(Enum.Parse<TipoDuracion>(s))
            );

            builder.Property(x => x.Duracion)
                .HasConversion(duracionConverter)
                .HasColumnName("duracion");

            builder.Property(x => x.ComidasPorDia).HasColumnName("comidasPorDia");

            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted").HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.HasMany(typeof(DiaDelPlan), "_diasDelPlan");
            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.DiasDelPlan);
        }

        public void Configure(EntityTypeBuilder<DiaDelPlan> builder)
        {
            builder.ToTable("DiaDelPlan");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.NumeroDia).HasColumnName("numeroDia");
            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted").HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.HasMany(typeof(TiempoDeComida), "_tiemposDeComida");
            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.TiemposDeComida);
        }

        public void Configure(EntityTypeBuilder<TiempoDeComida> builder)
        {
            builder.ToTable("TiempoDeComida");
            builder.HasKey(x => x.Id);
            builder.Property(x => x.Id).ValueGeneratedNever();
            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(200);
            builder.Property(x => x.Orden).HasColumnName("orden");
            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted").HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);
            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.RecetasAsignadas);

            builder.OwnsMany<AsignacionReceta>("_recetasAsignadas", asig =>
            {
                asig.Property<Guid>("Id").ValueGeneratedOnAdd();

                asig.Property(x => x.RecetaId)
                    .HasColumnName("recetaId");

                asig.OwnsOne(x => x.Racion, racion =>
                {
                    racion.Property(r => r.Cantidad).HasColumnName("racionCantidad");
                });
            });
        }
    }
}
