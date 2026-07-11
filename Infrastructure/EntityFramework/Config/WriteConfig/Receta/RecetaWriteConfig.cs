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

            var recipeNameConverter = new ValueConverter<RecipeName, string>(
                name => name.Name,
                s => (RecipeName)s
            );

            builder.Property(x => x.Id)
                .HasColumnName("id")
                .ValueGeneratedNever();

            builder.Property(x => x.Nombre)
                .HasConversion(recipeNameConverter)
                .HasColumnName("nombre")
                .HasMaxLength(500);

            builder.Property(x => x.Instrucciones)
                .HasColumnName("instrucciones")
                .HasMaxLength(4000);

            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted").HasDefaultValue(false);
            builder.HasQueryFilter(x => !x.IsDeleted);

            builder.Ignore(x => x.DomainEvents);
            builder.Ignore(x => x.Ingredientes);

            builder.OwnsMany<IngredienteReceta>("_ingredientes", ing =>
            {
                ing.Property<Guid>("Id").ValueGeneratedOnAdd();

                ing.Property(x => x.AlimentoId)
                    .HasColumnName("alimentoId");

                ing.OwnsOne(x => x.Porcion, porcion =>
                {
                    porcion.Property(p => p.Cantidad).HasColumnName("porcionCantidad").HasPrecision(12, 4);
                });
            });
        }
    }
}
