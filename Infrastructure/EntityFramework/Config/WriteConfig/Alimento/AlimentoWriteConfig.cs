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

            var alimentoIdConverter = new ValueConverter<AlimentoId, Guid>(
                id => id.Value,
                guid => AlimentoId.From(guid)
            );

            var alimentoNameConverter = new ValueConverter<AlimentoName, string>(
                name => name.Name,
                s => (AlimentoName)s
            );

            var categoriaNameConverter = new ValueConverter<CategoriaName, string>(
                name => name.Name,
                s => (CategoriaName)s
            );

            builder.Property(x => x.Id)
                .HasConversion(alimentoIdConverter)
                .HasColumnName("id");

            builder.Property(x => x.Nombre)
                .HasConversion(alimentoNameConverter)
                .HasColumnName("nombre")
                .HasMaxLength(500);

            builder.Property(x => x.Categoria)
                .HasConversion(categoriaNameConverter)
                .HasColumnName("categoria")
                .HasMaxLength(100);

            builder.OwnsOne(x => x.InfoNutricionalBase, info =>
            {
                info.Property(i => i.Gramos).HasColumnName("gramos").HasPrecision(12, 4);
                info.Property(i => i.Calorias).HasColumnName("calorias").HasPrecision(12, 4);
                info.Property(i => i.Proteinas).HasColumnName("proteinas").HasPrecision(12, 4);
                info.Property(i => i.Carbohidratos).HasColumnName("carbohidratos").HasPrecision(12, 4);
                info.Property(i => i.Grasas).HasColumnName("grasas").HasPrecision(12, 4);
            });

            builder.Ignore(x => x.DomainEvents);
        }
    }
}
