using Catalog.Domain.Factory.Receta;
using Catalog.Domain.Model.Recetas;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class RecetaFactoryTests
    {
        [Fact]
        public void Create_ValidData_CreatesRecetaWithInitialState()
        {
            // Arrange
            var factory = new RecetaFactory();

            // Act
            var receta = factory.Create("Ensalada", "Mezclar todos los ingredientes");

            // Assert
            Assert.IsType<Receta>(receta);
            Assert.NotEqual(Guid.Empty, receta.Id);
            Assert.Equal("Ensalada", receta.Nombre.Name);
            Assert.Equal("Mezclar todos los ingredientes", receta.Instrucciones);
            Assert.Empty(receta.Ingredientes);
            Assert.Single(receta.DomainEvents);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Create_EmptyOrWhitespaceInstructions_ThrowsException(string? instrucciones)
        {
            // Arrange
            var factory = new RecetaFactory();

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => factory.Create("Ensalada", instrucciones!));
            Assert.Equal("Recipe instructions cannot be empty", ex.Details);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_NullOrEmptyName_ThrowsException(string? nombre)
        {
            // Arrange
            var factory = new RecetaFactory();

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => factory.Create(nombre!, "Mezclar"));
            Assert.Equal("string cannot be null", ex.Details);
        }

        [Fact]
        public void Create_WhitespaceName_CreatesReceta()
        {
            // Arrange
            var factory = new RecetaFactory();

            // Act
            var receta = factory.Create("   ", "Mezclar");

            // Assert
            Assert.Equal("   ", receta.Nombre.Name);
        }
    }
}