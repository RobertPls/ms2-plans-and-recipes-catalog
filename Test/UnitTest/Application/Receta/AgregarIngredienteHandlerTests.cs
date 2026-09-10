using Catalog.Application.UseCase.Command.Receta.AgregarIngrediente;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class AgregarIngredienteHandlerTests
    {
        private readonly Mock<IRecetaRepository> _recetaRepoMock;
        private readonly Mock<IAlimentoRepository> _alimentoRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<AgregarIngredienteHandler>> _loggerMock;
        private readonly AgregarIngredienteHandler _handler;

        public AgregarIngredienteHandlerTests()
        {
            _recetaRepoMock = new Mock<IRecetaRepository>();
            _alimentoRepoMock = new Mock<IAlimentoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<AgregarIngredienteHandler>>();
            _handler = new AgregarIngredienteHandler(_recetaRepoMock.Object, _alimentoRepoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var manzana = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            var pollo = new Alimento(new AlimentoName("Pollo"), new CategoriaName("Proteina"), UnidadMedida.Gramo, new InfoNutricional(100, 165, 31m, 0m, 3.6m));
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            _alimentoRepoMock.Setup(a => a.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(manzana);
            _alimentoRepoMock.Setup(a => a.FindByIdAsync(It.Is<Guid>(id => id == pollo.Id))).ReturnsAsync(pollo);

            var command = new AgregarIngredienteCommand
            {
                RecetaId = receta.Id,
                Ingredientes = new List<IngredienteItem>
                {
                    new IngredienteItem { AlimentoId = manzana.Id, Cantidad = 200 },
                    new IngredienteItem { AlimentoId = pollo.Id, Cantidad = 100 }
                }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("2 ingrediente(s) agregado(s) exitosamente", result.Message);
            Assert.Equal(2, receta.Ingredientes.Count());
            _recetaRepoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == command.RecetaId)), Times.Once);
            _alimentoRepoMock.Verify(a => a.FindByIdAsync(It.IsAny<Guid>()), Times.Exactly(2));
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_RecetaNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Receta?)null);
            var command = new AgregarIngredienteCommand
            {
                RecetaId = Guid.NewGuid(),
                Ingredientes = new List<IngredienteItem> { new IngredienteItem { AlimentoId = Guid.NewGuid(), Cantidad = 1 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Receta no encontrada", result.Message);
            _alimentoRepoMock.Verify(a => a.FindByIdAsync(It.IsAny<Guid>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_AlimentoNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            _alimentoRepoMock.Setup(a => a.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Alimento?)null);
            var alimentoId = Guid.NewGuid();
            var command = new AgregarIngredienteCommand
            {
                RecetaId = receta.Id,
                Ingredientes = new List<IngredienteItem> { new IngredienteItem { AlimentoId = alimentoId, Cantidad = 1 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains($"El alimento con Id {alimentoId} no existe", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-1)]
        public async Task Handle_NonPositivePorcion_ReturnsFailureAndNoCommit(decimal cantidad)
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimento = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            _alimentoRepoMock.Setup(a => a.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(alimento);
            var command = new AgregarIngredienteCommand
            {
                RecetaId = receta.Id,
                Ingredientes = new List<IngredienteItem> { new IngredienteItem { AlimentoId = alimento.Id, Cantidad = cantidad } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Porcion cantidad must be greater than zero", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_DuplicateIngredient_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimento = new Alimento(new AlimentoName("Manzana"), new CategoriaName("Fruta"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            receta.AgregarIngrediente(alimento.Id, new Porcion(100));
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            _alimentoRepoMock.Setup(a => a.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(alimento);
            var command = new AgregarIngredienteCommand
            {
                RecetaId = receta.Id,
                Ingredientes = new List<IngredienteItem> { new IngredienteItem { AlimentoId = alimento.Id, Cantidad = 200 } }
            };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("ya está agregado", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}