using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.PlanAlimentario
{
    using PlanAlimentario = Catalog.Domain.Model.PlanesAlimentarios.PlanAlimentario;

    public class PlanAlimentarioFactory : IPlanAlimentarioFactory
    {
        public PlanAlimentario Create(string nombre, DuracionPlan duracion)
        {
            return new PlanAlimentario(nombre, duracion);
        }
    }
}
