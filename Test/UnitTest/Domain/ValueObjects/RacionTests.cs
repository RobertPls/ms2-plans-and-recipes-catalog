using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class RacionTests
    {
        [Fact]
        public void Constructor_PositiveQuantity_SetsValue()
        {
            // Act
            var racion = new Racion(2);

            // Assert
            Assert.Equal(2, racion.Cantidad);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void Constructor_NonPositiveQuantity_ThrowsException(int quantity)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new Racion(quantity));
            Assert.Equal("Racion must be greater than zero", ex.Details);
        }

        [Fact]
        public void Equality_SameQuantity_AreEqual()
        {
            // Arrange
            var a = new Racion(2);
            var b = new Racion(2);

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new Racion(3));
        }
    }
}