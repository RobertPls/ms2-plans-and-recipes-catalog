using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.PlanAlimentario
{
    public interface IPlanAlimentarioFactory
    {
        Model.PlanesAlimentarios.PlanAlimentario Create(string nombre, DuracionPlan duracion, DateTime fechaInicio);
    }
}
