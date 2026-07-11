using Shared.Core;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Event.Alimento;

namespace Catalog.Domain.Model.Alimentos
{
    public class Alimento : AggregateRoot<Guid>
    {
        public AlimentoName Nombre { get; private set; }
        public CategoriaName Categoria { get; private set; }
        public UnidadMedida UnidadMedida { get; private set; }
        public InfoNutricional InfoNutricionalBase { get; private set; }

        private Alimento() { Nombre = null!; Categoria = null!; InfoNutricionalBase = null!; }

        public Alimento(AlimentoName nombre, CategoriaName categoria, UnidadMedida unidadMedida, InfoNutricional infoNutricionalBase)
        {
            Id = Guid.NewGuid();
            Nombre = nombre;
            Categoria = categoria;
            UnidadMedida = unidadMedida;
            InfoNutricionalBase = infoNutricionalBase;

            AddDomainEvent(new AlimentoCreado(Id, Nombre, Categoria, InfoNutricionalBase));
        }

        public void ActualizarInfoNutricional(InfoNutricional info)
        {
            var anterior = InfoNutricionalBase;
            InfoNutricionalBase = info;

            AddDomainEvent(new AlimentoNutricionalActualizado(Id, anterior, info));
        }

        public void Actualizar(AlimentoName nombre, CategoriaName categoria, UnidadMedida unidadMedida, InfoNutricional info)
        {
            Nombre = nombre;
            Categoria = categoria;
            UnidadMedida = unidadMedida;
            var anterior = InfoNutricionalBase;
            InfoNutricionalBase = info;

            AddDomainEvent(new AlimentoNutricionalActualizado(Id, anterior, info));
        }
    }
}
