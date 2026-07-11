namespace Catalog.Infrastructure.EntityFramework.ReadModel.PlanAlimentario
{
    public class PlanAlimentarioReadModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public string DuracionTipo { get; set; } = null!;
        public int ComidasPorDia { get; set; }
        public List<DiaDelPlanReadModel> DiasDelPlan { get; set; } = new();
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
        public bool IsDeleted { get; set; }
    }

    public class DiaDelPlanReadModel
    {
        public Guid Id { get; set; }
        public int NumeroDia { get; set; }
        public Guid PlanAlimentarioId { get; set; }
        public PlanAlimentarioReadModel PlanAlimentario { get; set; } = null!;
        public List<TiempoDeComidaReadModel> TiemposDeComida { get; set; } = new();
    }

    public class TiempoDeComidaReadModel
    {
        public Guid Id { get; set; }
        public string Nombre { get; set; } = null!;
        public int Orden { get; set; }
        public Guid DiaDelPlanId { get; set; }
        public DiaDelPlanReadModel DiaDelPlan { get; set; } = null!;
        public List<RecetaAsignadaReadModel> RecetasAsignadas { get; set; } = new();
    }

    public class RecetaAsignadaReadModel
    {
        public Guid Id { get; set; }
        public Guid RecetaId { get; set; }
        public decimal RacionCantidad { get; set; }
        public Guid TiempoDeComidaId { get; set; }
        public TiempoDeComidaReadModel TiempoDeComida { get; set; } = null!;
    }
}
