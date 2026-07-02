using Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record Porcion : ValueObject
    {
        public decimal Cantidad { get; }
        public string Unidad { get; }

        private Porcion() { Unidad = null!; }

        public Porcion(decimal cantidad, string unidad)
        {
            if (cantidad <= 0)
                throw new BussinessRuleValidationException("Porcion cantidad must be greater than zero");
            if (string.IsNullOrWhiteSpace(unidad))
                throw new BussinessRuleValidationException("Porcion unidad cannot be empty");
            Cantidad = cantidad;
            Unidad = unidad;
        }
    }
}
