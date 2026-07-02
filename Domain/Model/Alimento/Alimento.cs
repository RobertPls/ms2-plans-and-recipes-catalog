using Shared.Core;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Event.Alimento;

namespace Catalog.Domain.Model.Alimentos
{
    public class Alimento : AggregateRoot<AlimentoId>
    {
        public AlimentoName Nombre { get; private set; }
        public CategoriaName Categoria { get; private set; }
        public InfoNutricional InfoNutricionalBase { get; private set; }

        private Alimento() { Nombre = null!; Categoria = null!; InfoNutricionalBase = null!; }

        public Alimento(AlimentoName nombre, CategoriaName categoria, InfoNutricional infoNutricionalBase)
        {
            Id = AlimentoId.New();
            Nombre = nombre;
            Categoria = categoria;
            InfoNutricionalBase = infoNutricionalBase;

            AddDomainEvent(new AlimentoCreado(Id, Nombre, Categoria, InfoNutricionalBase));
        }

        public void ActualizarInfoNutricional(InfoNutricional info)
        {
            var anterior = InfoNutricionalBase;
            InfoNutricionalBase = info;

            AddDomainEvent(new AlimentoNutricionalActualizado(Id, anterior, info));
        }
    }
}
