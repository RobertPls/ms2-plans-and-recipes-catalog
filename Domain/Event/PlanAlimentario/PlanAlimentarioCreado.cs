using Catalog.Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Event.PlanAlimentario
{
    public record PlanAlimentarioCreado : DomainEvent
    {
        public PlanId PlanId { get; }
        public string Nombre { get; }
        public string TipoDuracion { get; }
        public int DiasTotales { get; }
        public DateTime FechaInicio { get; }

        public PlanAlimentarioCreado(PlanId planId, string nombre, string tipoDuracion, int diasTotales, DateTime fechaInicio)
            : base(DateTime.UtcNow)
        {
            PlanId = planId;
            Nombre = nombre;
            TipoDuracion = tipoDuracion;
            DiasTotales = diasTotales;
            FechaInicio = fechaInicio;
        }
    }
}
