using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.Alimento
{
    public class AlimentoFactory : IAlimentoFactory
    {
        public Model.Alimentos.Alimento Create(string nombre, string categoria, InfoNutricional infoNutricional)
        {
            return new Model.Alimentos.Alimento(nombre, categoria, infoNutricional);
        }
    }
}
