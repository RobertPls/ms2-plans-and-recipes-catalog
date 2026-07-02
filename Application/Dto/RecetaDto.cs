namespace Catalog.Application.Dto
{
    public class RecetaDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string Instrucciones { get; set; } = null!;
        public List<IngredienteDto> Ingredientes { get; set; } = new();
    }

    public class IngredienteDto
    {
        public Guid AlimentoId { get; set; }
        public string NombreAlimento { get; set; } = null!;
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; } = null!;
    }
}
