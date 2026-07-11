using Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record InfoNutricional : ValueObject
    {
        public decimal Cantidad { get; }
        public decimal Calorias { get; }
        public decimal Proteinas { get; }
        public decimal Carbohidratos { get; }
        public decimal Grasas { get; }

        public InfoNutricional(decimal cantidad, decimal calorias, decimal proteinas, decimal carbohidratos, decimal grasas)
        {
            if (cantidad <= 0)
                throw new BussinessRuleValidationException("InfoNutricional cantidad must be greater than zero");
            Cantidad = cantidad;
            Calorias = calorias;
            Proteinas = proteinas;
            Carbohidratos = carbohidratos;
            Grasas = grasas;
        }
    }
}
