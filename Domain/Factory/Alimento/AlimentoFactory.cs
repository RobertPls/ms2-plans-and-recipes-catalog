using Catalog.Domain.ValueObjects;

namespace Catalog.Domain.Factory.Alimento
{
    using Alimento = Catalog.Domain.Model.Alimentos.Alimento;

    public class AlimentoFactory : IAlimentoFactory
    {
        public Alimento Create(string nombre, string categoria, InfoNutricional infoNutricional)
        {
            return new Alimento(nombre, categoria, infoNutricional);
        }
    }
}
