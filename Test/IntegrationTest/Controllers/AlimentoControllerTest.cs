using Catalog.Domain.Repository.Alimento;
using Catalog.Infrastructure.EntityFramework.Context;
using Catalog.Infrastructure.EntityFramework.ReadModel.Alimento;
using Catalog.Tests.IntegrationTest.Setup;
using Microsoft.Extensions.DependencyInjection;
using System.Net;
using System.Net.Http.Json;
using Xunit;

namespace Catalog.Tests.IntegrationTest.Controllers
{
    public class AlimentoControllerTest : IClassFixture<AlimentoControllerWebApplicationFactory>
    {
        private readonly AlimentoControllerWebApplicationFactory _factory;

        public AlimentoControllerTest(AlimentoControllerWebApplicationFactory factory)
        {
            _factory = factory;
        }

        private static object NuevaAlimento(string nombre, int unidadMedida = 1) => new
        {
            Nombre = nombre,
            Categoria = "Frutas",
            UnidadMedida = unidadMedida,
            Cantidad = 100m,
            Calorias = 52m,
            Proteinas = 0.3m,
            Carbohidratos = 14m,
            Grasas = 0.2m
        };

        [Fact]
        public async Task Create_WithValidRequest_PersistsAlimento()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/alimentos", NuevaAlimento("Manzana"));

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<AlimentoCreateResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotEqual(Guid.Empty, body.Data);

            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAlimentoRepository>();
            var alimento = await repository.FindByIdAsync(body.Data);
            Assert.NotNull(alimento);
            Assert.Equal("MANZANA", alimento!.Nombre.Name);
            Assert.Equal(52m, alimento.InfoNutricionalBase.Calorias);
        }

        [Fact]
        public async Task Create_WithEmptyName_ReturnsBadRequest()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/alimentos", NuevaAlimento(""));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task Create_WithInvalidUnidadMedida_ReturnsBadRequest()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PostAsJsonAsync("/api/v1/alimentos", NuevaAlimento("Manzana", unidadMedida: 999));

            //Assert
            Assert.Equal(HttpStatusCode.BadRequest, response.StatusCode);
        }

        [Fact]
        public async Task GetById_AfterCreate_ReturnsAlimento()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createResponse = await client.PostAsJsonAsync("/api/v1/alimentos", NuevaAlimento("Papa"));
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<AlimentoCreateResponse>();

            //Act
            var response = await client.GetAsync($"/api/v1/alimentos/{created!.Data}");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<AlimentoGetResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Equal("PAPA", body.Data.Nombre);
        }

        [Fact]
        public async Task GetById_WhenAlimentoDoesNotExist_Returns404()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.GetAsync($"/api/v1/alimentos/{Guid.NewGuid()}");

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }

        [Fact]
        public async Task GetByCategoria_WithSeededReadModel_ReturnsAlimentos()
        {
            //Arrange
            var client = _factory.CreateClient();
            using (var scope = _factory.Services.CreateScope())
            {
                var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                readDb.Alimento.Add(new AlimentoReadModel
                {
                    Id = Guid.NewGuid(),
                    Nombre = "MANZANA",
                    Categoria = "FRUTAS",
                    UnidadMedida = 1,
                    Cantidad = 100m,
                    Calorias = 52m,
                    Proteinas = 0.3m,
                    Carbohidratos = 14m,
                    Grasas = 0.2m,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await readDb.SaveChangesAsync();
            }

            //Act
            var response = await client.GetAsync("/api/v1/alimentos/categoria/FRUTAS?page=1&pageSize=10");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<AlimentoListResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Contains(body.Data.Items, a => a.Nombre == "MANZANA");
        }

        [Fact]
        public async Task List_WithSeededReadModel_ReturnsAlimentos()
        {
            //Arrange
            var client = _factory.CreateClient();
            using (var scope = _factory.Services.CreateScope())
            {
                var readDb = scope.ServiceProvider.GetRequiredService<ReadDbContext>();
                readDb.Alimento.Add(new AlimentoReadModel
                {
                    Id = Guid.NewGuid(),
                    Nombre = "BANANA",
                    Categoria = "FRUTAS",
                    UnidadMedida = 1,
                    Cantidad = 100m,
                    Calorias = 89m,
                    Proteinas = 1.1m,
                    Carbohidratos = 23m,
                    Grasas = 0.3m,
                    CreatedAt = DateTime.UtcNow,
                    IsDeleted = false
                });
                await readDb.SaveChangesAsync();
            }

            //Act
            var response = await client.GetAsync("/api/v1/alimentos?page=1&pageSize=10");

            //Assert
            Assert.Equal(HttpStatusCode.OK, response.StatusCode);
            var body = await response.Content.ReadFromJsonAsync<AlimentoListResponse>();
            Assert.NotNull(body);
            Assert.True(body!.Success);
            Assert.NotNull(body.Data);
            Assert.Contains(body.Data.Items, a => a.Nombre == "BANANA");
        }

        [Fact]
        public async Task Update_WithValidRequest_UpdatesAlimento()
        {
            //Arrange
            var client = _factory.CreateClient();
            var createResponse = await client.PostAsJsonAsync("/api/v1/alimentos", NuevaAlimento("Leche"));
            createResponse.EnsureSuccessStatusCode();
            var created = await createResponse.Content.ReadFromJsonAsync<AlimentoCreateResponse>();

            //Act
            var updateResponse = await client.PutAsJsonAsync("/api/v1/alimentos", new
            {
                AlimentoId = created!.Data,
                Nombre = "Pera",
                Categoria = "Frutas",
                UnidadMedida = 1,
                Cantidad = 120m,
                Calorias = 60m,
                Proteinas = 0.4m,
                Carbohidratos = 15m,
                Grasas = 0.1m
            });

            //Assert
            Assert.Equal(HttpStatusCode.OK, updateResponse.StatusCode);
            var body = await updateResponse.Content.ReadFromJsonAsync<Envelope>();
            Assert.NotNull(body);
            Assert.True(body!.Success);

            using var scope = _factory.Services.CreateScope();
            var repository = scope.ServiceProvider.GetRequiredService<IAlimentoRepository>();
            var alimento = await repository.FindByIdAsync(created.Data);
            Assert.NotNull(alimento);
            Assert.Equal("PERA", alimento!.Nombre.Name);
            Assert.Equal(60m, alimento.InfoNutricionalBase.Calorias);
        }

        [Fact]
        public async Task Update_WithUnknownId_Returns404()
        {
            //Arrange
            var client = _factory.CreateClient();

            //Act
            var response = await client.PutAsJsonAsync("/api/v1/alimentos", new
            {
                AlimentoId = Guid.NewGuid(),
                Nombre = "Pera",
                Categoria = "Frutas",
                UnidadMedida = 1,
                Cantidad = 120m,
                Calorias = 60m,
                Proteinas = 0.4m,
                Carbohidratos = 15m,
                Grasas = 0.1m
            });

            //Assert
            Assert.Equal(HttpStatusCode.NotFound, response.StatusCode);
        }
    }
}