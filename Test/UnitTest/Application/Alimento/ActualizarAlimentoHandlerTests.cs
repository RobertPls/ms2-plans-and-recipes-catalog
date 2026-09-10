using Catalog.Application.UseCase.Command.Alimento.ActualizarAlimento;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class ActualizarAlimentoHandlerTests
    {
        private readonly Mock<IAlimentoRepository> _repoMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<ActualizarAlimentoHandler>> _loggerMock;
        private readonly ActualizarAlimentoHandler _handler;

        public ActualizarAlimentoHandlerTests()
        {
            _repoMock = new Mock<IAlimentoRepository>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<ActualizarAlimentoHandler>>();
            _handler = new ActualizarAlimentoHandler(_repoMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        private static ActualizarAlimentoCommand CreateValidCommand(Guid alimentoId) =>
            new()
            {
                AlimentoId = alimentoId,
                Nombre = "Manzana Roja",
                Categoria = "Fruta",
                UnidadMedida = (int)UnidadMedida.Kilogramo,
                Cantidad = 100,
                Calorias = 60,
                Proteinas = 0.5m,
                Carbohidratos = 16,
                Grasas = 0.3m
            };

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("MANZANA"), new CategoriaName("FRUTA"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(alimento);
            var command = CreateValidCommand(alimento.Id);

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal("Alimento actualizado exitosamente", result.Message);
            Assert.Equal("MANZANA ROJA", alimento.Nombre.Name);
            Assert.Equal(UnidadMedida.Kilogramo, alimento.UnidadMedida);
            Assert.Equal(60m, alimento.InfoNutricionalBase.Calorias);
            _repoMock.Verify(r => r.FindByIdAsync(It.Is<Guid>(id => id == alimento.Id)), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Fact]
        public async Task Handle_AlimentoNotFound_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync((Alimento?)null);
            var command = CreateValidCommand(Guid.NewGuid());

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Alimento no encontrado", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        public async Task Handle_InvalidUnitOfMeasure_ReturnsFailureAndNoCommit(int unidadMedida)
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("MANZANA"), new CategoriaName("FRUTA"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(alimento);
            var command = CreateValidCommand(alimento.Id);
            command.UnidadMedida = unidadMedida;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unidad de medida inválida", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-5)]
        public async Task Handle_NonPositiveCantidad_ReturnsFailureAndNoCommit(decimal cantidad)
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("MANZANA"), new CategoriaName("FRUTA"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            _repoMock.Setup(r => r.FindByIdAsync(It.IsAny<Guid>())).ReturnsAsync(alimento);
            var command = CreateValidCommand(alimento.Id);
            command.Cantidad = cantidad;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InfoNutricional cantidad must be greater than zero", result.Message);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}