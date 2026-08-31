using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class PlanNameTests
    {
        [Fact]
        public void Constructor_ValidName_SetsValue()
        {
            // Act
            var nombre = new PlanName("Plan Saludable");

            // Assert
            Assert.Equal("Plan Saludable", nombre.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_NullOrEmptyName_ThrowsException(string? name)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new PlanName(name!));
            Assert.Equal("string cannot be null", ex.Details);
        }

        [Fact]
        public void Constructor_MoreThan200Characters_ThrowsException()
        {
            // Arrange
            var longName = new string('a', 201);

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new PlanName(longName));
            Assert.Equal("Plan name cannot be more than 200 characters", ex.Details);
        }

        [Fact]
        public void Constructor_WhitespaceName_DoesNotThrow()
        {
            // Act
            var nombre = new PlanName("   ");

            // Assert
            Assert.Equal("   ", nombre.Name);
        }

        [Fact]
        public void Equality_SameName_AreEqual()
        {
            // Arrange
            var a = new PlanName("Plan Saludable");
            var b = new PlanName("Plan Saludable");

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new PlanName("Plan Deportivo"));
        }

        [Fact]
        public void ImplicitConversion_StringToName_Works()
        {
            // Act
            PlanName nombre = "Plan Saludable";

            // Assert
            Assert.Equal("Plan Saludable", nombre.Name);
            string valor = nombre;
            Assert.Equal("Plan Saludable", valor);
        }
    }
}