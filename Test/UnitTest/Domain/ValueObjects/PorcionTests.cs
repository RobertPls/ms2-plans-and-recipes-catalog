using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class PorcionTests
    {
        [Fact]
        public void Constructor_PositiveQuantity_SetsValue()
        {
            // Act
            var porcion = new Porcion(2.5m);

            // Assert
            Assert.Equal(2.5m, porcion.Cantidad);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_NonPositiveQuantity_ThrowsException(decimal quantity)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new Porcion(quantity));
            Assert.Equal("Porcion cantidad must be greater than zero", ex.Details);
        }

        [Fact]
        public void Equality_SameQuantity_AreEqual()
        {
            // Arrange
            var a = new Porcion(2m);
            var b = new Porcion(2m);

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new Porcion(3m));
        }
    }
}