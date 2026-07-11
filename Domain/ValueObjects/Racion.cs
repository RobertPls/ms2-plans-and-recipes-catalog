using Shared.Core;

namespace Catalog.Domain.ValueObjects
{
    public record Racion : ValueObject
    {
        public int Cantidad { get; }

        private Racion() { }

        public Racion(int cantidad)
        {
            if (cantidad <= 0)
                throw new BussinessRuleValidationException("Racion must be greater than zero");
            Cantidad = cantidad;
        }
    }
}
