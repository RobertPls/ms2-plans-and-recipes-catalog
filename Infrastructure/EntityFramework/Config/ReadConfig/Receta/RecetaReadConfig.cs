using Catalog.Infrastructure.EntityFramework.ReadModel.Receta;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntityFramework.Config.ReadConfig.Receta
{
    public class RecetaReadConfig : IEntityTypeConfiguration<RecetaReadModel>,
        IEntityTypeConfiguration<IngredienteRecetaReadModel>
    {
        public void Configure(EntityTypeBuilder<RecetaReadModel> builder)
        {
            builder.ToTable("Receta");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(500);
            builder.Property(x => x.Instrucciones).HasColumnName("instrucciones").HasMaxLength(4000);

            builder.HasMany(x => x.Ingredientes).WithOne(x => x.Receta);
        }

        public void Configure(EntityTypeBuilder<IngredienteRecetaReadModel> builder)
        {
            builder.ToTable("IngredienteReceta");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.AlimentoId).HasColumnName("alimentoId");
            builder.Property(x => x.PorcionCantidad).HasColumnName("porcionCantidad").HasPrecision(12, 4);
            builder.Property(x => x.PorcionUnidad).HasColumnName("porcionUnidad").HasMaxLength(50);
        }
    }
}
