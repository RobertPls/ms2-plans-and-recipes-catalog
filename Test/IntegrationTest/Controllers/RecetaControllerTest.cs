using Catalog.Domain.Repository.Receta;
using Catalog.Infrastructure.EntityFramework.Context;
using Catalog.Infrastructure.EntityFramework.ReadModel.Receta;
using Catalog.Tests.IntegrationTest.Setup;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Catalog.Tests.IntegrationTest.Controllers
{
    public class RecetaControllerTest : IClassFixture<RecetaControllerWebApplicationFactory>
    {
        private readonly RecetaControllerWebApplicationFactory _factory;

        public RecetaControllerTest(RecetaControllerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private static object NuevaReceta() => new
        {
            Nombre = "Salmon al horno",
            Instrucciones = "Hornear 25 minutos"
        };

        [Fact]
        public async Task Create_WithValidRequest_PersistsReceta()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/recetas", NuevaReceta());

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<RecetaCreateResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotEqual(Guid.Empty, body.Data);

            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRecetaRepository>();
            var receta = await repository.FindByIdAsync(body.Data);
            Assert.NotNull(receta);
            Assert.Equal("Salmon al horno", receta!.Nombre.Name);
            Assert.Empty(receta.Ingredientes);
        }

        [Theory]
        [InlineData("")]
        [InlineData("   ")]
        public async Task Create_WithEmptyInstructions_ReturnsBadRequest(string instrucciones)
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/recetas", new
            {
                Nombre = "Ensalada",
                Instrucciones = instrucciones
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetById_AfterCreate_ReturnsReceta()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createResponse = await client.PostAsJsonAsync("/api/v1/recetas", NuevaReceta());
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<RecetaCreateResponse>();

            //Act
            var response = await client.GetAsync($"/api/v1/recetas/{created!.Data}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<RecetaGetResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Equal("Salmon al horno", body.Data.Nombre);
        }

        [Fact]
        public async Task GetById_WhenRecetaDoesNotExist_Returns404()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"/api/v1/recetas/{Guid.NewGuid()}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task AgregarIngrediente_WithValidRequest_AddsIngredient()
        {
            //Arrange
            var client = _factory.CreateClient();
            var recetaResponse = await client.PostAsJsonAsync("/api/v1/recetas", NuevaReceta());
            recetaResponse.EnsureSuccessStatusCode();
            var recetaCreated = await recetaResponse.Content.ReadFromJsonAsync<RecetaCreateResponse>();

            var alimentoResponse = await client.PostAsJsonAsync("/api/v1/alimentos", new
            {
                Nombre = "Manzana",
                Categoria = "Frutas",
                UnidadMedida = 1,
                Cantidad = 100m,
                Calorias = 52m,
                Proteinas = 0.3m,
                Carbohidratos = 14m,
                Grasas = 0.2m
            });
            alimentoResponse.EnsureSuccessStatusCode();
            var alimentoCreated = await alimentoResponse.Content.ReadFromJsonAsync<AlimentoCreateResponse>();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/recetas/ingredientes", new
            {
                RecetaId = recetaCreated!.Data,
                Ingredientes = new[]
                {
                    new { AlimentoId = alimentoCreated!.Data, Cantidad = 150 }
                }
            });

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<Envelope>();
            Assert.NotNull(body);
            Assert.True(body!.Success);

            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IRecetaRepository>();
            var receta = await repository.FindByIdAsync(recetaCreated.Data);
            Assert.NotNull(receta);
            Assert.Contains(receta!.Ingredientes, i => i.AlimentoId == alimentoCreated.Data);
        }

        [Fact]
        public async Task AgregarIngrediente_WithUnknownAlimento_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();
            var recetaResponse = await client.PostAsJsonAsync("/api/v1/recetas", NuevaReceta());
            recetaResponse.EnsureSuccessStatusCode();
            var recetaCreated = await recetaResponse.Content.ReadFromJsonAsync<RecetaCreateResponse>();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/recetas/ingredientes", new
            {
                RecetaId = recetaCreated!.Data,
                Ingredientes = new[]
                {
                    new { AlimentoId = Guid.NewGuid(), Cantidad = 150 }
                }
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task AgregarIngrediente_WhenRecetaDoesNotExist_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/recetas/ingredientes", new
            {
                RecetaId = Guid.NewGuid(),
                Ingredientes = new[]
                {
                    new { AlimentoId = Guid.NewGuid(), Cantidad = 150 }
                }
            });

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task RemoverIngrediente_WhenRecetaDoesNotExist_Returns400()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.DeleteAsync($"/api/v1/recetas/{Guid.NewGuid()}/ingredientes/{Guid.NewGuid()}");

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetInfoNutricional_WhenRecetaDoesNotExist_Returns404()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"/api/v1/recetas/{Guid.NewGuid()}/info-nutricional");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task List_WithSeededReadModel_ReturnsRecetas()
        {
            //Arrange
            var client = _factory.CreateClient();
            using (var scope = _factory.Services.CreateScope())
            {
                var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                readDb.Receta.Add(new RecetaReadModel
                {
                    Id = Guid.NewGuid(),
                    Nombre = "Sopa de verduras",
                    Instrucciones = "Cocinar 20 minutos",
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await readDb.SaveChangesAsync();
            }

            //Act
            var response = await client.GetAsync("/api/v1/recetas?page=1&pageSize=10");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<RecetaListResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Contains(body.Data.Items, r => r.Nombre == "Sopa de verduras");
        }
    }
}