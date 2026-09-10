using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Infrastructure.EntityFramework.Context;
using Catalog.Infrastructure.EntityFramework.ReadModel.PlanAlimentario;
using Catalog.Tests.IntegrationTest.Setup;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Catalog.Tests.IntegrationTest.Controllers
{
    public class PlanAlimentarioControllerTest : IClassFixture<PlanAlimentarioControllerWebApplicationFactory>
    {
        private readonly PlanAlimentarioControllerWebApplicationFactory _factory;

        public PlanAlimentarioControllerTest(PlanAlimentarioControllerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private static object NuevoPlan() => new
        {
            Nombre = "Dieta equilibrada",
            DuracionTipo = "QUINCENAL",
            ComidasPorDia = 3
        };

        [Fact]
        public async Task Create_WithValidRequest_PersistsPlan()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/planes", NuevoPlan());

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<PlanCreateResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotEqual(Guid.Empty, body.Data);

            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IPlanAlimentarioRepository>();
            var plan = await repository.FindByIdAsync(body.Data);
            Assert.NotNull(plan);
            Assert.Equal("Dieta equilibrada", plan!.Nombre.Name);
            Assert.Equal(3, plan.ComidasPorDia);
            Assert.Equal(15, plan.DiasDelPlan.Count());
        }

        [Fact]
        public async Task Create_WithInvalidDuracion_ReturnsBadRequest()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/planes", new
            {
                Nombre = "Dieta semanal",
                DuracionTipo = "SEMANAL",
                ComidasPorDia = 3
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AgregarTiempoComida_WhenPlanDoesNotExist_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/planes/tiempos-comida", new
            {
                PlanId = Guid.NewGuid(),
                NumDia = 1,
                Tipo = 1
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AsignarReceta_WhenPlanDoesNotExist_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/planes/asignar-recetas", new
            {
                PlanId = Guid.NewGuid(),
                TiempoComidaId = Guid.NewGuid(),
                Recetas = new[]
                {
                    new { RecetaId = Guid.NewGuid(), RacionCantidad = 10 }
                }
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoverRecetaDeTiempo_WhenPlanDoesNotExist_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.DeleteAsync(
                $"/api/v1/planes/{Guid.NewGuid()}/tiempos-comida/{Guid.NewGuid()}/recetas/{Guid.NewGuid()}");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetComposicion_AfterCreate_ReturnsComposicion()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createResponse = await client.PostAsJsonAsync("/api/v1/planes", NuevoPlan());
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<PlanCreateResponse>();

            //Act
            var response = await client.GetAsync($"/api/v1/planes/{created!.Data}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<Envelope>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
        }

        [Fact]
        public async Task GetComposicion_WhenPlanDoesNotExist_Returns404()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"/api/v1/planes/{Guid.NewGuid()}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task List_WithSeededReadModel_ReturnsPlanes()
        {
            //Arrange
            var client = _factory.CreateClient();
            using (var scope = _factory.Services.CreateScope())
            {
                var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                readDb.PlanAlimentario.Add(new PlanAlimentarioReadModel
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Plan deportista",
                    DuracionTipo = "QUINCENAL",
                    ComidasPorDia = 5,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await readDb.SaveChangesAsync();
            }

            //Act
            var response = await client.GetAsync("/api/v1/planes");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<PlanListResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Contains(body.Data, p => p.Nombre == "Plan deportista");
        }
    }
}