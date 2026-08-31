using Catalog.Application.UseCase.Command.Receta.RemoverIngrediente;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class RemoverIngredienteHandlerTests
    {
        private readonly Mock<IRecetaRepository> _recetaRepoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<RemoverIngredienteHandler>> _loggerMock;
        private readonly RemoverIngredienteHandler _handler;

        public RemoverIngredienteHandlerTests()
        {
            _recetaRepoMock = new Mock<IRecetaRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<RemoverIngredienteHandler>>();
            _handler = new RemoverIngredienteHandler(_recetaRepoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            var alimentoId = Guid.NewGuid();
            receta.AgregarIngrediente(alimentoId, new Porcion(2));
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            var command = new RemoverIngredienteCommand { RecetaId = receta.Id, AlimentoId = alimentoId };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Ingrediente removido exitosamente", result.Message);
            Assert.Empty(receta.Ingredientes);
            _recetaRepoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == command.RecetaId)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_RecetaNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Receta?)null);
            var command = new RemoverIngredienteCommand { RecetaId = Guid.NewGuid(), AlimentoId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Receta no encontrada", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_IngredientNotInReceta_ReturnsFailureAndNoCommit()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar");
            _recetaRepoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(receta);
            var command = new RemoverIngredienteCommand { RecetaId = receta.Id, AlimentoId = Guid.NewGuid() };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("no existe en la receta", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}