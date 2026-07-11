using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.Alimento
{
    using Alimento = Catalog.Domain.Model.Alimentos.Alimento;

    public interface IAlimentoFactory
    {
        Alimento Create(string nombre, string categoria, UnidadMedida unidadMedida, InfoNutricional infoNutricional);
    }
}
