using Catalog.Domain;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class DuracionPlanTests
    {
        [Theory]
        [InlineData(TipoDuracion.QUINCENAL, 15)]
        [InlineData(TipoDuracion.MENSUAL, 30)]
        public void Constructor_ValidDuration_SetsTipoAndDays(TipoDuracion tipo, int expectedDays)
        {
            // Act
            var duracion = new DuracionPlan(tipo);

            // Assert
            Assert.Equal(tipo, duracion.Tipo);
            Assert.True(duracion.EsValida());
            Assert.Equal(expectedDays, duracion.Dias());
        }

        [Fact]
        public void Constructor_InvalidDuration_ThrowsException()
        {
            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() => new DuracionPlan((TipoDuracion)99));
            Assert.Equal("DuracionPlan must be QUINCENAL or MENSUAL", ex.Details);
        }

        [Fact]
        public void Equality_SameTipo_AreEqual()
        {
            // Arrange
            var a = new DuracionPlan(TipoDuracion.QUINCENAL);
            var b = new DuracionPlan(TipoDuracion.QUINCENAL);

            // Assert
            Assert.Equal(a, b);
            Assert.NotEqual(a, new DuracionPlan(TipoDuracion.MENSUAL));
        }
    }
}