using Catalog.Domain.Model.Alimentos;
using Shared.Core;
using Catalog.Domain.ValueObjects;
using Catalog.Domain.Event.Receta;

namespace Catalog.Domain.Model.Recetas
{
    public class Receta : AggregateRoot<RecetaId>
    {
        public RecipeName Nombre { get; private set; }
        public string Instrucciones { get; private set; }
        private readonly ICollection<IngredienteReceta> _ingredientes;

        public IEnumerable<IngredienteReceta> Ingredientes => _ingredientes;

        private Receta() { Nombre = null!; Instrucciones = null!; _ingredientes = null!; }

        public Receta(RecipeName nombre, string instrucciones)
        {
            if (string.IsNullOrWhiteSpace(instrucciones))
                throw new BussinessRuleValidationException("Recipe instructions cannot be empty");

            Id = RecetaId.New();
            Nombre = nombre;
            Instrucciones = instrucciones;
            _ingredientes = new List<IngredienteReceta>();

            AddDomainEvent(new RecetaCreada(Id, Nombre));
        }

        public void AgregarIngrediente(AlimentoId alimentoId, Porcion porcion)
        {
            var ingrediente = new IngredienteReceta(alimentoId, porcion);
            _ingredientes.Add(ingrediente);

            AddDomainEvent(new IngredienteAgregadoAReceta(Id, alimentoId, porcion.Cantidad));
        }

        public void RemoverIngrediente(AlimentoId alimentoId)
        {
            var ingrediente = _ingredientes.FirstOrDefault(i => i.AlimentoId == alimentoId);
            if (ingrediente == null)
                throw new BussinessRuleValidationException($"El ingrediente {alimentoId.Value} no existe en la receta");

            _ingredientes.Remove(ingrediente);

            AddDomainEvent(new IngredienteRemovidoDeReceta(Id, alimentoId, ingrediente.Porcion.Cantidad));
        }

        public InfoNutricional CalcularInfoNutricionalTotal(Func<AlimentoId, Alimento> obtenerAlimento)
        {
            decimal totalCalorias = 0, totalProteinas = 0, totalCarbos = 0, totalGrasas = 0;

            foreach (var ing in _ingredientes)
            {
                var alimento = obtenerAlimento(ing.AlimentoId);
                if (alimento == null) continue;

                var factor = ing.Porcion.Cantidad / alimento.InfoNutricionalBase.Cantidad;
                totalCalorias += alimento.InfoNutricionalBase.Calorias * factor;
                totalProteinas += alimento.InfoNutricionalBase.Proteinas * factor;
                totalCarbos += alimento.InfoNutricionalBase.Carbohidratos * factor;
                totalGrasas += alimento.InfoNutricionalBase.Grasas * factor;
            }

            return new InfoNutricional(1, totalCalorias, totalProteinas, totalCarbos, totalGrasas);
        }
    }
}
