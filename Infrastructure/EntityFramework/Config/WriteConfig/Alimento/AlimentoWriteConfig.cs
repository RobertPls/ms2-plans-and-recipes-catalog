using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.Infrastructure.EntityFramework.Config.WriteConfig.Alimentos
{
    public class AlimentoWriteConfig : IEntityTypeConfiguration<Alimento>
    {
        public void Configure(EntityTypeBuilder<Alimento> builder)
        {
            builder.ToTable("Alimento");
            builder.HasKey(x => x.Id);

            var alimentoNameConverter = new ValueConverter<AlimentoName, string>(
                name => name.Name,
                s => (AlimentoName)s
            );

            var categoriaNameConverter = new ValueConverter<CategoriaName, string>(
                name => name.Name,
                s => (CategoriaName)s
            );

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.Nombre)
                .HasConversion(alimentoNameConverter)
                .HasColumnName("nombre")
                .HasMaxLength(500);

            builder.Property(x => x.Categoria)
                .HasConversion(categoriaNameConverter)
                .HasColumnName("categoria")
                .HasMaxLength(100);

            builder.Property(x => x.UnidadMedida)
                .HasColumnName("unidadMedida")
                .HasConversion<int>();

            builder.OwnsOne(x => x.InfoNutricionalBase, info =>
            {
                info.Property(i => i.Cantidad).HasColumnName("cantidad").HasPrecision(12, 4);
                info.Property(i => i.Calorias).HasColumnName("calorias").HasPrecision(12, 4);
                info.Property(i => i.Proteinas).HasColumnName("proteinas").HasPrecision(12, 4);
                info.Property(i => i.Carbohidratos).HasColumnName("carbohidratos").HasPrecision(12, 4);
                info.Property(i => i.Grasas).HasColumnName("grasas").HasPrecision(12, 4);
            });

            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted").HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.Ignore(x => x.DomainEvents);
        }
    }
}
