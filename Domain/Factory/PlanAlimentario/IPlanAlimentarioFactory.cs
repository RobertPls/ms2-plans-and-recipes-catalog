using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.PlanAlimentario
{
    using PlanAlimentario = Catalog.Domain.Model.PlanesAlimentarios.PlanAlimentario;

    public interface IPlanAlimentarioFactory
    {
        PlanAlimentario Create(string nombre, DuracionPlan duracion, DateTime fechaInicio);
    }
}
