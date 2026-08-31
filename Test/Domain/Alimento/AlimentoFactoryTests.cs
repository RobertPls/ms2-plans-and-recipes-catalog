using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class AlimentoFactoryTests
    {
        [Fact]
        public void Create_ValidData_CreatesAlimentoWithInitialState()
        {
            // Arrange
            var factory = new AlimentoFactory();
            var info = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);

            // Act
            var alimento = factory.Create("Manzana", "Fruta", UnidadMedida.Gramo, info);

            // Assert
            Assert.IsType<Alimento>(alimento);
            Assert.NotEqual(Guid.Empty, alimento.Id);
            Assert.Equal("Manzana", alimento.Nombre.Name);
            Assert.Equal("Fruta", alimento.Categoria.Name);
            Assert.Equal(UnidadMedida.Gramo, alimento.UnidadMedida);
            Assert.Equal(info, alimento.InfoNutricionalBase);
            Assert.Single(alimento.DomainEvents);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_NullOrEmptyName_ThrowsException(string? nombre)
        {
            // Arrange
            var factory = new AlimentoFactory();

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                factory.Create(nombre!, "Fruta", UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m)));
            Assert.Equal("string cannot be null", ex.Details);
        }

        [Fact]
        public void Create_WhitespaceName_CreatesAlimento()
        {
            // Arrange
            var factory = new AlimentoFactory();

            // Act
            var alimento = factory.Create("   ", "Fruta", UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));

            // Assert
            Assert.Equal("   ", alimento.Nombre.Name);
        }
    }
}