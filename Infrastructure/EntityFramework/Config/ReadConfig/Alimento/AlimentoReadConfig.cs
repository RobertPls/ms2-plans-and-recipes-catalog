using Catalog.Infrastructure.EntityFramework.ReadModel.Alimento;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Catalog.Infrastructure.EntityFramework.Config.ReadConfig.Alimento
{
    public class AlimentoReadConfig : IEntityTypeConfiguration<AlimentoReadModel>
    {
        public void Configure(EntityTypeBuilder<AlimentoReadModel> builder)
        {
            builder.ToTable("Alimento");
            builder.HasKey(x => x.Id);

            builder.Property(x => x.Nombre).HasColumnName("nombre").HasMaxLength(500);
            builder.Property(x => x.Categoria).HasColumnName("categoria").HasMaxLength(100);
            builder.Property(x => x.UnidadMedida).HasColumnName("unidadMedida");
            builder.Property(x => x.Cantidad).HasColumnName("cantidad").HasPrecision(12, 4);
            builder.Property(x => x.Calorias).HasColumnName("calorias").HasPrecision(12, 4);
            builder.Property(x => x.Proteinas).HasColumnName("proteinas").HasPrecision(12, 4);
            builder.Property(x => x.Carbohidratos).HasColumnName("carbohidratos").HasPrecision(12, 4);
            builder.Property(x => x.Grasas).HasColumnName("grasas").HasPrecision(12, 4);
            builder.Property(x => x.CreatedAt).HasColumnName("createdAt");
            builder.Property(x => x.UpdatedAt).HasColumnName("updatedAt");
            builder.Property(x => x.DeletedAt).HasColumnName("deletedAt");
            builder.Property(x => x.IsDeleted).HasColumnName("isDeleted");
        }
    }
}
