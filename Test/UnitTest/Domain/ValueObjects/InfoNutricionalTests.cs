using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class InfoNutricionalTests
    {
        [Fact]
        public void Constructor_ValidData_SetsAllValues()
        {
            // Act
            var info = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);

            // Assert
            Assert.Equal(100m, info.Cantidad);
            Assert.Equal(52m, info.Calorias);
            Assert.Equal(0.3m, info.Proteinas);
            Assert.Equal(14m, info.Carbohidratos);
            Assert.Equal(0.2m, info.Grasas);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-0.5)]
        [InlineData(-100)]
        public void Constructor_NonPositiveCantidad_ThrowsException(decimal cantidad)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                new InfoNutricional(cantidad, 52, 0.3m, 14, 0.2m));
            Assert.Equal("InfoNutricional cantidad must be greater than zero", ex.Details);
        }

        [Fact]
        public void Equality_SameValues_AreEqual()
        {
            // Arrange
            var a = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);
            var b = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new InfoNutricional(100, 60, 0.3m, 14, 0.2m));
        }
    }
}