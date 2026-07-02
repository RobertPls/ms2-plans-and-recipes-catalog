using Catalog.Domain.ValueObjects;

namespace Catalog.Application.Dto
{
    public class PlanAlimentarioDto
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string DuracionTipo { get; set; } = null!;
        public int DiasTotal { get; set; }
        public DateTime FechaInicio { get; set; }
        public List<DiaDelPlanDto> Dias { get; set; } = new();
    }

    public class DiaDelPlanDto
    {
        public int NumeroDia { get; set; }
        public List<TiempoDeComidaDto> Tiempos { get; set; } = new();
    }

    public class TiempoDeComidaDto
    {
        public string Nombre { get; set; } = null!;
        public int Orden { get; set; }
        public List<RecetaAsignadaDto> Recetas { get; set; } = new();
    }

    public class RecetaAsignadaDto
    {
        public Guid RecetaId { get; set; }
        public string NombreReceta { get; set; } = null!;
        public decimal Racion { get; set; }
    }
}
