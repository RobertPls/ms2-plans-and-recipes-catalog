using Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record Porcion : ValueObject
    {
        public decimal Cantidad { get; }

        private Porcion() { }

        public Porcion(decimal cantidad)
        {
            if (cantidad <= 0)
                throw new BussinessRuleValidationException("Porcion cantidad must be greater than zero");
            Cantidad = cantidad;
        }
    }
}
