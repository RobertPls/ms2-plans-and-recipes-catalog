using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Query.PlanAlimentario
{
    public class GetComposicionPlanQuery : IRequest<ComposicionPlanDto?>
    {
        public Guid PlanId { get; set; }
        public GetComposicionPlanQuery() { }
        public GetComposicionPlanQuery(Guid planId) => PlanId = planId;
    }

    public class ComposicionPlanDto
    {
        public Guid PlanId { get; set; }
        public List<ComposicionDiaDto> Dias { get; set; } = new();
    }

    public class ComposicionDiaDto
    {
        public int NumeroDia { get; set; }
        public List<ComposicionTiempoDto> TiemposDeComida { get; set; } = new();
    }

    public class ComposicionTiempoDto
    {
        public Guid TiempoDeComidaId { get; set; }
        public string Nombre { get; set; } = null!;
        public int Orden { get; set; }
        public List<ComposicionRecetaDto> Recetas { get; set; } = new();
    }

    public class ComposicionRecetaDto
    {
        public Guid RecetaId { get; set; }
        public string NombreReceta { get; set; } = null!;
        public RacionDto Racion { get; set; } = null!;
        public List<ComposicionIngredienteDto> Ingredientes { get; set; } = new();
    }

    public class RacionDto
    {
        public decimal Cantidad { get; set; }
    }

    public class ComposicionIngredienteDto
    {
        public Guid AlimentoId { get; set; }
        public string NombreAlimento { get; set; } = null!;
        public PorcionDto Porcion { get; set; } = null!;
    }

    public class PorcionDto
    {
        public decimal Cantidad { get; set; }
        public string Unidad { get; set; } = null!;
    }
}
