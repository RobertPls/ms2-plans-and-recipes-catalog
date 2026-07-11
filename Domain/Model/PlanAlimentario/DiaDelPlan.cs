using Catalog.Domain.ValueObjects;
using Shared.Core;

namespace Catalog.Domain.Model.PlanesAlimentarios
{
    public class DiaDelPlan : Entity<Guid>
    {
        public int NumeroDia { get; private set; }
        private readonly ICollection<TiempoDeComida> _tiemposDeComida = new List<TiempoDeComida>();

        public IEnumerable<TiempoDeComida> TiemposDeComida => _tiemposDeComida;

        internal DiaDelPlan(int numeroDia)
        {
            Id = Guid.NewGuid();
            NumeroDia = numeroDia;
        }

        internal void AgregarTiempoDeComida(TipoTiempoComida tipo)
        {
            var orden = (int)tipo;
            if (_tiemposDeComida.Any(t => t.Orden == orden))
                throw new BussinessRuleValidationException($"Ya existe un tiempo de comida con orden {orden} en el día {NumeroDia}");

            var tiempo = new TiempoDeComida(tipo.ToDisplayName(), orden);
            _tiemposDeComida.Add(tiempo);
        }

        internal TiempoDeComida? BuscarTiempoDeComida(Guid tId)
        {
            return _tiemposDeComida.FirstOrDefault(t => t.Id == tId);
        }
    }
}
