namespace Catalog.Infrastructure.EntityFramework.ReadModel.Receta
{
    public class RecetaReadModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Instrucciones { get; set; } = null!;
        public List<IngredienteRecetaReadModel> Ingredientes { get; set; } = new();
    }

    public class IngredienteRecetaReadModel
    {
        public Guid Id { get; set; }
        public Guid AlimentoId { get; set; }
        public decimal PorcionCantidad { get; set; }
        public string PorcionUnidad { get; set; } = null!;
        public Guid RecetaId { get; set; }
        public RecetaReadModel Receta { get; set; } = null!;
    }
}
