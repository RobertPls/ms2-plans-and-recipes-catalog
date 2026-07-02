using Catalog.Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record InfoNutricional : ValueObject
    {
        public decimal Gramos { get; }
        public decimal Calorias { get; }
        public decimal Proteinas { get; }
        public decimal Carbohidratos { get; }
        public decimal Grasas { get; }

        public InfoNutricional(decimal gramos, decimal calorias, decimal proteinas, decimal carbohidratos, decimal grasas)
        {
            if (gramos <= 0)
                throw new BussinessRuleValidationException("InfoNutricional gramos must be greater than zero");
            Gramos = gramos;
            Calorias = calorias;
            Proteinas = proteinas;
            Carbohidratos = carbohidratos;
            Grasas = grasas;
        }
    }
}
