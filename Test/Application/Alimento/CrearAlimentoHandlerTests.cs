using Catalog.Application.UseCase.Command.Alimento.CrearAlimento;
using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Model.Alimentos;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class CrearAlimentoHandlerTests
    {
        private readonly Mock<IAlimentoRepository> _repoMock;
        private readonly Mock<IAlimentoFactory> _factoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CrearAlimentoHandler>> _loggerMock;
        private readonly CrearAlimentoHandler _handler;

        public CrearAlimentoHandlerTests()
        {
            _repoMock = new Mock<IAlimentoRepository>();
            _factoryMock = new Mock<IAlimentoFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CrearAlimentoHandler>>();
            _handler = new CrearAlimentoHandler(_repoMock.Object, _factoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        private static CrearAlimentoCommand CreateValidCommand() =>
            new()
            {
                Nombre = "Manzana",
                Categoria = "Fruta",
                UnidadMedida = (int)UnidadMedida.Gramo,
                Cantidad = 100,
                Calorias = 52,
                Proteinas = 0.3m,
                Carbohidratos = 14,
                Grasas = 0.2m
            };

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var alimento = new Alimento(new AlimentoName("MANZANA"), new CategoriaName("FRUTA"), UnidadMedida.Gramo, new InfoNutricional(100, 52, 0.3m, 14, 0.2m));
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UnidadMedida>(), It.IsAny<InfoNutricional>()))
                .Returns(alimento);
            var command = CreateValidCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(alimento.Id, result.Value);
            Assert.Equal("Alimento creado exitosamente", result.Message);
            _factoryMock.Verify(f => f.Create("MANZANA", "FRUTA", UnidadMedida.Gramo, It.Is<InfoNutricional>(i => i.Calorias == 52m)), Times.Once);
            _repoMock.Verify(r => r.CreateAsync(It.Is<Alimento>(a => a.Nombre.Name == "MANZANA")), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(99)]
        [InlineData(-1)]
        public async Task Handle_InvalidUnitOfMeasure_ReturnsFailureAndNoCommit(int unidadMedida)
        {
            // Arrange
            var command = CreateValidCommand();
            command.UnidadMedida = unidadMedida;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Contains("Unidad de medida inválida", result.Message);
            _factoryMock.Verify(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UnidadMedida>(), It.IsAny<InfoNutricional>()), Times.Never);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Alimento>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Theory]
        [InlineData(0)]
        [InlineData(-10)]
        public async Task Handle_NonPositiveCantidad_ReturnsFailureAndNoCommit(decimal cantidad)
        {
            // Arrange
            var command = CreateValidCommand();
            command.Cantidad = cantidad;

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("InfoNutricional cantidad must be greater than zero", result.Message);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Alimento>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }

        [Fact]
        public async Task Handle_FactoryThrows_ReturnsFailureAndNoCommit()
        {
            // Arrange
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>(), It.IsAny<UnidadMedida>(), It.IsAny<InfoNutricional>()))
                .Throws(new BussinessRuleValidationException("Alimento name cannot be null"));
            var command = CreateValidCommand();

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Alimento name cannot be null", result.Message);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Alimento>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
        }
    }
}