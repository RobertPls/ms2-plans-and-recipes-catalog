using Shared.Core;
using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Model.PlanesAlimentarios
{
    public class TiempoDeComida : Entity<Guid>
    {
        public string Nombre { get; private set; } = null!;
        public int Orden { get; private set; }
        private readonly ICollection<AsignacionReceta> _recetasAsignadas = new List<AsignacionReceta>();

        public IEnumerable<AsignacionReceta> RecetasAsignadas => _recetasAsignadas;

        internal TiempoDeComida(string nombre, int orden)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Orden = orden;
        }

        internal void AsignarReceta(RecetaId recetaId, Racion racion)
        {
            if (_recetasAsignadas.Any(r => r.RecetaId == recetaId))
                throw new BussinessRuleValidationException($"La receta {recetaId.Value} ya está asignada a este tiempo de comida");

            var asignacion = new AsignacionReceta(recetaId, racion);
            _recetasAsignadas.Add(asignacion);
        }

        internal void RemoverReceta(RecetaId recetaId)
        {
            var asignacion = _recetasAsignadas.FirstOrDefault(r => r.RecetaId == recetaId);
            if (asignacion == null)
                throw new BussinessRuleValidationException($"La receta {recetaId.Value} no está asignada a este tiempo de comida");

            _recetasAsignadas.Remove(asignacion);
        }
    }
}
