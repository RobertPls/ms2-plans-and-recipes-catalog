using Catalog.Domain.Model.Recetas;
using Catalog.Domain.ValueObjects;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace Catalog.Infrastructure.EntityFramework.Config.WriteConfig.Recetas
{
    public class RecetaWriteConfig : IEntityTypeConfiguration<Receta>
    {
        public void Configure(EntityTypeBuilder<Receta> builder)
        {
            builder.ToTable("Receta");
            builder.HasKey(x => x.Id);

            var recetaIdConverter = new ValueConverter<RecetaId, Guid>(
                id => id.Value,
                guid => RecetaId.From(guid)
            );

            var recipeNameConverter = new ValueConverter<RecipeName, string>(
                name => name.Name,
                s => (RecipeName)s
            );

            builder.Property(x => x.Id)
                .HasConversion(recetaIdConverter)
                .HasColumnName("id");

            builder.Property(x => x.Nombre)
                .HasConversion(recipeNameConverter)
                .HasColumnName("nombre")
                .HasMaxLength(500);

            builder.Property(x => x.Instrucciones)
                .HasColumnName("instrucciones")
                .HasMaxLength(4000);

            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.Ingredientes);

            builder.OwnsMany<IngredienteReceta>("_ingredientes", ing =>
            {
                var alimentoIdConverter = new ValueConverter<AlimentoId, Guid>(
                    id => id.Value,
                    guid => AlimentoId.From(guid)
                );

                ing.Property<Guid>("Id").ValueGeneratedOnAdd();

                ing.Property(x => x.AlimentoId)
                    .HasConversion(alimentoIdConverter)
                    .HasColumnName("alimentoId");

                ing.OwnsOne(x => x.Porcion, porcion =>
                {
                    porcion.Property(p => p.Cantidad).HasColumnName("porcionCantidad").HasPrecision(12, 4);
                    porcion.Property(p => p.Unidad).HasColumnName("porcionUnidad").HasMaxLength(50);
                });
            });
        }
    }
}
