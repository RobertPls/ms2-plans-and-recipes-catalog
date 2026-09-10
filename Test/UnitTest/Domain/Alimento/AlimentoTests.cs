using Catalog.Domain.Event.Alimento;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.ValueObjects;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class AlimentoTests
    {
        [Fact]
        public void Constructor_ValidData_CreatesAlimentoWithInitialState()
        {
            // Arrange
            var nombre = new AlimentoName("Manzana");
            var categoria = new CategoriaName("Fruta");
            var info = new InfoNutricional(100, 52, 0.3m, 14, 0.2m);

            // Act
            var alimento = new Alimento(nombre, categoria, UnidadMedida.Gramo, info);

            // Assert
            Assert.NotEqual(Guid.Empty, alimento.Id);
            Assert.Equal("Manzana", alimento.Nombre.Name);
            Assert.Equal("Fruta", alimento.Categoria.Name);
            Assert.Equal(UnidadMedida.Gramo, alimento.UnidadMedida);
            Assert.Equal(52m, alimento.InfoNutricionalBase.Calorias);
            var evento = Assert.IsType<AlimentoCreado>(Assert.Single(alimento.DomainEvents));
            Assert.Equal(alimento.Id, evento.AlimentoId);
        }

        [Fact]
        public void ActualizarInfoNutricional_ValidInfo_UpdatesAndQueuesEvent()
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            var nueva = new InfoNutricional(100, 60, 0.5m, 16, 0.3m);

            // Act
            alimento.ActualizarInfoNutricional(nueva);

            // Assert
            Assert.Equal(60m, alimento.InfoNutricionalBase.Calorias);
            var evento = Assert.IsType<AlimentoNutricionalActualizado>(alimento.DomainEvents.Last());
            Assert.Equal(52m, evento.InfoNutricionalAnterior.Calorias);
            Assert.Equal(60m, evento.InfoNutricionalNueva.Calorias);
        }

        [Fact]
        public void Actualizar_ValidData_UpdatesAllFieldsAndQueuesEvent()
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));

            // Act
            alimento.Actualizar(new AlimentoName("Manzana Roja"), new CategoriaName("Fruta de temporada"), UnidadMedida.Kilogramo, new InfoNutricional(100, 60, 0.5m, 16, 0.3m));

            // Assert
            Assert.Equal("Manzana Roja", alimento.Nombre.Name);
            Assert.Equal("Fruta de temporada", alimento.Categoria.Name);
            Assert.Equal(UnidadMedida.Kilogramo, alimento.UnidadMedida);
            Assert.Equal(60m, alimento.InfoNutricionalBase.Calorias);
            Assert.Equal(2, alimento.DomainEvents.Count);
            Assert.IsType<AlimentoNutricionalActualizado>(alimento.DomainEvents.Last());
        }
    }
}