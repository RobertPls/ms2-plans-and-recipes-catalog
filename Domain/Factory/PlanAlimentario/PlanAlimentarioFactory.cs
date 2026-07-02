using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.PlanAlimentario
{
    public class PlanAlimentarioFactory : IPlanAlimentarioFactory
    {
        public Model.PlanesAlimentarios.PlanAlimentario Create(string nombre, DuracionPlan duracion, DateTime fechaInicio)
        {
            return new Model.PlanesAlimentarios.PlanAlimentario(nombre, duracion, fechaInicio);
        }
    }
}
