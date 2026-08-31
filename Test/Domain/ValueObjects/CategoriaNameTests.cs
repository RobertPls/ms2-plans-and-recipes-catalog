using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class CategoriaNameTests
    {
        [Fact]
        public void Constructor_ValidName_SetsValue()
        {
            // Act
            var categoria = new CategoriaName("Fruta");

            // Assert
            Assert.Equal("Fruta", categoria.Name);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Constructor_NullOrEmptyName_ThrowsException(string? name)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new CategoriaName(name!));
            Assert.Equal("string cannot be null", ex.Details);
        }

        [Fact]
        public void Constructor_MoreThan50Characters_ThrowsException()
        {
            // Arrange
            var longName = new string('a', 51);

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new CategoriaName(longName));
            Assert.Equal("Categoria name cannot be more than 50 characters", ex.Details);
        }

        [Fact]
        public void Constructor_WhitespaceName_DoesNotThrow()
        {
            // Act
            var categoria = new CategoriaName("   ");

            // Assert
            Assert.Equal("   ", categoria.Name);
        }

        [Fact]
        public void Equality_SameName_AreEqual()
        {
            // Arrange
            var a = new CategoriaName("Fruta");
            var b = new CategoriaName("Fruta");

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new CategoriaName("Verdura"));
        }

        [Fact]
        public void ImplicitConversion_StringToName_Works()
        {
            // Act
            CategoriaName categoria = "Fruta";

            // Assert
            Assert.Equal("Fruta", categoria.Name);
            string valor = categoria;
            Assert.Equal("Fruta", valor);
        }
    }
}