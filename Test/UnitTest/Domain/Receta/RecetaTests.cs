using Catalog.Domain.Event.Receta;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class RecetaTests
    {
        [Fact]
        public void Constructor_ValidData_CreatesRecetaWithInitialState()
        {
            // Arrange
            var nombre = new RecipeName("Ensalada");

            // Act
            var receta = new Receta(nombre, "Mezclar todos los ingredientes");

            // Assert
            Assert.NotEqual(Guid.Empty, receta.Id);
            Assert.Equal("Ensalada", receta.Nombre.Name);
            Assert.Equal("Mezclar todos los ingredientes", receta.Instrucciones);
            Assert.Empty(receta.Ingredientes);
            var evento = Assert.IsType<RecetaCreada>(Assert.Single(receta.DomainEvents));
            Assert.Equal(receta.Id, evento.RecetaId);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        [InlineData("   ")]
        public void Constructor_EmptyOrWhitespaceInstructions_ThrowsException(string? instrucciones)
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                new Receta(new RecipeName("Ensalada"), instrucciones!));
            Assert.Equal("Recipe instructions cannot be empty", ex.Details);
        }

        [Fact]
        public void AgregarIngrediente_ValidIngredient_AddsAndQueuesEvent()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimentoId = Guid.NewGuid();

            // Act
            receta.AgregarIngrediente(alimentoId, new Porcion(2));

            // Assert
            var ingrediente = Assert.Single(receta.Ingredientes);
            Assert.Equal(alimentoId, ingrediente.AlimentoId);
            Assert.Equal(2m, ingrediente.Porcion.Cantidad);
            var evento = Assert.IsType<IngredienteAgregadoAReceta>(receta.DomainEvents.Last());
            Assert.Equal(alimentoId, evento.AlimentoId);
            Assert.Equal(2m, evento.PorcionCantidad);
        }

        [Fact]
        public void AgregarIngrediente_DuplicateIngredient_ThrowsException()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimentoId = Guid.NewGuid();
            receta.AgregarIngrediente(alimentoId, new Porcion(1));

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                receta.AgregarIngrediente(alimentoId, new Porcion(2)));
            Assert.Contains("ya está agregado", ex.Details);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        [InlineData(-100)]
        public void AgregarIngrediente_NonPositiveQuantity_ThrowsException(decimal quantity)
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");

            // Act + Assert
            Assert.Throws<BussinessRuleValidationException>(() =>
                receta.AgregarIngrediente(Guid.NewGuid(), new Porcion(quantity)));
        }

        [Fact]
        public void RemoverIngrediente_ExistingIngredient_RemovesAndQueuesEvent()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimentoId = Guid.NewGuid();
            receta.AgregarIngrediente(alimentoId, new Porcion(2));

            // Act
            receta.RemoverIngrediente(alimentoId);

            // Assert
            Assert.Empty(receta.Ingredientes);
            var evento = Assert.IsType<IngredienteRemovidoDeReceta>(receta.DomainEvents.Last());
            Assert.Equal(alimentoId, evento.AlimentoId);
            Assert.Equal(2m, evento.PorcionCantidad);
        }

        [Fact]
        public void RemoverIngrediente_NonExistingIngredient_ThrowsException()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                receta.RemoverIngrediente(Guid.NewGuid()));
            Assert.Contains("no existe en la receta", ex.Details);
        }

        [Theory]
        [InlineData(2, 104)]
        [InlineData(1, 52)]
        [InlineData(0.5, 26)]
        public void CalcularInfoNutricional_DifferentPortions_CalculatesCalories(decimal portion, decimal expected)
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimento = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(1, 52, 0.3m, 14, 0.2m));
            receta.AgregarIngrediente(alimento.Id, new Porcion(portion));

            // Act
            var info = receta.CalcularInfoNutricionalTotal(_ => alimento);

            // Assert
            Assert.Equal(expected, info.Calorias);
        }

        [Fact]
        public void CalcularInfoNutricional_WithMultipleIngredients_CalculatesTotals()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var manzana = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            var pollo = new Alimento(new AlimentoName("Pollo"), new CategoriaName("Proteina"), UnidadMedida.Gramo, new InfoNutricional(100, 165, 31m, 0m, 3.6m));
            receta.AgregarIngrediente(manzana.Id, new Porcion(200));
            receta.AgregarIngrediente(pollo.Id, new Porcion(100));

            // Act
            var info = receta.CalcularInfoNutricionalTotal(id => id == manzana.Id ? manzana : pollo);

            // Assert
            Assert.Equal(269m, info.Calorias);
            Assert.Equal(31.6m, info.Proteinas);
            Assert.Equal(28m, info.Carbohidratos);
            Assert.Equal(4m, info.Grasas);
        }

        [Fact]
        public void CalcularInfoNutricional_WhenAlimentoIsNull_SkipsIngredient()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            receta.AgregarIngrediente(Guid.NewGuid(), new Porcion(2));

            // Act
            var info = receta.CalcularInfoNutricionalTotal(_ => null!);

            // Assert
            Assert.Equal(0m, info.Calorias);
            Assert.Equal(0m, info.Proteinas);
            Assert.Equal(0m, info.Carbohidratos);
            Assert.Equal(0m, info.Grasas);
        }
    }
}