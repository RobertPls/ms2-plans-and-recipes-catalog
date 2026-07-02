using Shared.Core;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Event.PlanAlimentario;

namespace Catalog.Domain.Model.PlanesAlimentarios
{
    public class PlanAlimentario : AggregateRoot<PlanId>
    {
        public PlanName Nombre { get; private set; }
        public DuracionPlan Duracion { get; private set; }
        public DateTime FechaInicio { get; private set; }
        private readonly ICollection<DiaDelPlan> _diasDelPlan;

        public IEnumerable<DiaDelPlan> DiasDelPlan => _diasDelPlan;

        private PlanAlimentario() { Nombre = null!; Duracion = null!; _diasDelPlan = null!; }

        public PlanAlimentario(PlanName nombre, DuracionPlan duracion, DateTime fechaInicio)
        {
            if (!duracion.EsValida())
                throw new BussinessRuleValidationException("Invalid plan duration");
            if (!ValidarInicioPermitido(fechaInicio))
                throw new BussinessRuleValidationException("Start date must be at least 1 day in the future");

            Id = PlanId.New();
            Nombre = nombre;
            Duracion = duracion;
            FechaInicio = fechaInicio;
            _diasDelPlan = new List<DiaDelPlan>();

            for (int i = 1; i <= duracion.Dias(); i++)
            {
                _diasDelPlan.Add(new DiaDelPlan(i));
            }

            AddDomainEvent(new PlanAlimentarioCreado(Id, Nombre, Duracion.Tipo.ToString(), Duracion.Dias(), FechaInicio));
        }

        public void AgregarTiempoDeComidaADia(int numDia, string nombre, int orden)
        {
            var dia = _diasDelPlan.FirstOrDefault(d => d.NumeroDia == numDia);
            if (dia == null)
                throw new BussinessRuleValidationException($"Día {numDia} no existe en el plan");

            dia.AgregarTiempoDeComida(nombre, orden);
        }

        public void AsignarRecetaATiempo(int numDia, Guid tId, RecetaId recetaId, Racion racion)
        {
            var dia = _diasDelPlan.FirstOrDefault(d => d.NumeroDia == numDia);
            if (dia == null)
                throw new BussinessRuleValidationException($"Día {numDia} no existe en el plan");

            var tiempo = dia.BuscarTiempoDeComida(tId);
            if (tiempo == null)
                throw new BussinessRuleValidationException($"Tiempo de comida {tId} no existe en el día {numDia}");

            tiempo.AsignarReceta(recetaId, racion);

            AddDomainEvent(new RecetaAsignadaATiempo(Id, numDia, tId, recetaId, racion.Cantidad));
        }

        public static bool ValidarInicioPermitido(DateTime fechaInicio)
        {
            return fechaInicio.Date > DateTime.UtcNow.Date;
        }
    }
}
