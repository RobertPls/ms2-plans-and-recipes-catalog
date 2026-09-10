using Catalog.Domain;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class PlanAlimentarioFactoryTests
    {
        [Theory]
        [InlineData(TipoDuracion.QUINCENAL, 15)]
        [InlineData(TipoDuracion.MENSUAL, 30)]
        public void Create_ValidDuration_CreatesPlanWithExpectedDays(TipoDuracion tipo, int expectedDays)
        {
            // Arrange
            var factory = new PlanAlimentarioFactory();
            var duracion = new DuracionPlan(tipo);

            // Act
            var plan = factory.Create("Plan Saludable", duracion, 3);

            // Assert
            Assert.IsType<PlanAlimentario>(plan);
            Assert.NotEqual(Guid.Empty, plan.Id);
            Assert.Equal("Plan Saludable", plan.Nombre.Name);
            Assert.Equal(3, plan.ComidasPorDia);
            Assert.Equal(expectedDays, plan.DiasDelPlan.Count());
            Assert.Single(plan.DomainEvents);
        }

        [Theory]
        [InlineData(null)]
        [InlineData("")]
        public void Create_NullOrEmptyName_ThrowsException(string? nombre)
        {
            // Arrange
            var factory = new PlanAlimentarioFactory();
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);

            // Act + Assert
            var ex = Assert.Throws<BussinessRuleValidationException>(() =>
                factory.Create(nombre!, duracion, 3));
            Assert.Equal("string cannot be null", ex.Details);
        }

        [Fact]
        public void Create_WhitespaceName_CreatesPlan()
        {
            // Arrange
            var factory = new PlanAlimentarioFactory();
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);

            // Act
            var plan = factory.Create("   ", duracion, 3);

            // Assert
            Assert.Equal("   ", plan.Nombre.Name);
        }

        [Fact]
        public void Create_ZeroComidasPorDia_CreatesPlan()
        {
            // Arrange
            var factory = new PlanAlimentarioFactory();
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);

            // Act
            var plan = factory.Create("Plan", duracion, 0);

            // Assert
            Assert.Equal(0, plan.ComidasPorDia);
        }
    }
}