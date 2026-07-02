using Catalog.Shared.Core;
using Catalog.Domain;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Model.PlanesAlimentarios;
using Catalog.Domain.ValueObjects;
using Xunit;

namespace Catalog.Tests.Domain
{
    public class PlanAlimentarioFactoryTests
    {
        private readonly IPlanAlimentarioFactory _factory;

        public PlanAlimentarioFactoryTests()
        {
            _factory = new PlanAlimentarioFactory();
        }

        [Fact]
        public void Create_ValidPlan_ReturnsPlanWithCorrectProperties()
        {
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            var fechaInicio = DateTime.UtcNow.AddDays(1);
            var plan = _factory.Create("Plan Prueba", duracion, fechaInicio);

            Assert.Equal("Plan Prueba", plan.Nombre);
            Assert.Equal(TipoDuracion.QUINCENAL, plan.Duracion.Tipo);
            Assert.Equal(fechaInicio, plan.FechaInicio);
            Assert.Equal(15, plan.DiasDelPlan.Count());
        }

        [Fact]
        public void Create_MensualPlan_Creates30Days()
        {
            var duracion = new DuracionPlan(TipoDuracion.MENSUAL);
            var plan = _factory.Create("Plan Mensual", duracion, DateTime.UtcNow.AddDays(2));

            Assert.Equal(30, plan.DiasDelPlan.Count());
        }

        [Fact]
        public void Create_EmptyName_ThrowsException()
        {
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            Assert.Throws<BussinessRuleValidationException>(() =>
                _factory.Create("", duracion, DateTime.UtcNow.AddDays(1)));
        }

        [Fact]
        public void Create_PastStartDate_ThrowsException()
        {
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            Assert.Throws<BussinessRuleValidationException>(() =>
                _factory.Create("Plan", duracion, DateTime.UtcNow.AddDays(-1)));
        }

        [Fact]
        public void Create_AgregarTiempoComida_Works()
        {
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            var plan = _factory.Create("Plan Con Comidas", duracion, DateTime.UtcNow.AddDays(1));

            plan.AgregarTiempoDeComidaADia(1, "Desayuno", 1);
            plan.AgregarTiempoDeComidaADia(1, "Almuerzo", 2);

            var dia1 = plan.DiasDelPlan.First(d => d.NumeroDia == 1);
            Assert.Equal(2, dia1.TiemposDeComida.Count());
        }

        [Fact]
        public void Create_AsignarRecetaATiempo_Works()
        {
            var duracion = new DuracionPlan(TipoDuracion.QUINCENAL);
            var plan = _factory.Create("Plan", duracion, DateTime.UtcNow.AddDays(1));
            plan.AgregarTiempoDeComidaADia(1, "Desayuno", 1);

            var recetaId = RecetaId.New();
            var racion = new Racion(2);
            var dia1 = plan.DiasDelPlan.First(d => d.NumeroDia == 1);
            var tId = dia1.TiemposDeComida.First().Id;

            plan.AsignarRecetaATiempo(1, tId, recetaId, racion);

            var tiempo = dia1.TiemposDeComida.First();
            Assert.Single(tiempo.RecetasAsignadas);
        }
    }
}
