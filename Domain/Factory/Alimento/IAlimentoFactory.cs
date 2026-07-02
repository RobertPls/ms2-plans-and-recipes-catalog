using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.Alimento
{
    public interface IAlimentoFactory
    {
        Model.Alimentos.Alimento Create(string nombre, string categoria, InfoNutricional infoNutricional);
    }
}
