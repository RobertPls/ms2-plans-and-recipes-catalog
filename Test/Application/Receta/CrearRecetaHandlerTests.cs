using Catalog.Application.UseCase.Command.Receta.CrearReceta;
using Catalog.Domain.Factory.Receta;
using Catalog.Domain.Model.Recetas;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;
using Moq;
using Shared.Core;
using Xunit;

namespace Catalog.Tests.Application
{
    public class CrearRecetaHandlerTests
    {
        private readonly Mock<IRecetaRepository> _repoMock;
        private readonly Mock<IRecetaFactory> _factoryMock;
        private readonly Mock<IUnitOfWork> _unitOfWorkMock;
        private readonly Mock<ILogger<CrearRecetaHandler>> _loggerMock;
        private readonly CrearRecetaHandler _handler;

        public CrearRecetaHandlerTests()
        {
            _repoMock = new Mock<IRecetaRepository>();
            _factoryMock = new Mock<IRecetaFactory>();
            _unitOfWorkMock = new Mock<IUnitOfWork>();
            _loggerMock = new Mock<ILogger<CrearRecetaHandler>>();
            _handler = new CrearRecetaHandler(_repoMock.Object, _factoryMock.Object, _unitOfWorkMock.Object, _loggerMock.Object);
        }

        [Fact]
        public async Task Handle_ValidCommand_ReturnsSuccessAndVerifiesInteractions()
        {
            // Arrange
            var receta = new Receta(new RecipeName("Ensalada"), "Mezclar todos los ingredientes");
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>())).Returns(receta);
            var command = new CrearRecetaCommand { Nombre = "Ensalada", Instrucciones = "Mezclar todos los ingredientes" };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.True(result.IsSuccess);
            Assert.Equal(receta.Id, result.Value);
            Assert.Equal("Receta creada exitosamente", result.Message);
            _factoryMock.Verify(f => f.Create("Ensalada", "Mezclar todos los ingredientes"), Times.Once);
            _repoMock.Verify(r => r.CreateAsync(It.Is<Receta>(r2 => r2.Nombre.Name == "Ensalada")), Times.Once);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Once);
        }

        [Theory]
        [InlineData("", "")]
        [InlineData(null, "Mezclar")]
        [InlineData("Ensalada", "   ")]
        [InlineData("Ensalada", "")]
        public async Task Handle_DomainValidationFails_FactoryThrows_ReturnsFailureAndNoCommit(string? nombre, string? instrucciones)
        {
            // Arrange
            _factoryMock.Setup(f => f.Create(It.IsAny<string>(), It.IsAny<string>()))
                .Throws(new BussinessRuleValidationException("Recipe instructions cannot be empty"));
            var command = new CrearRecetaCommand { Nombre = nombre!, Instrucciones = instrucciones! };

            // Act
            var result = await _handler.Handle(command, CancellationToken.None);

            // Assert
            Assert.False(result.IsSuccess);
            Assert.Equal("Recipe instructions cannot be empty", result.Message);
            _repoMock.Verify(r => r.CreateAsync(It.IsAny<Receta>()), Times.Never);
            _unitOfWorkMock.Verify(u => u.Commit(), Times.Never);
            _loggerMock.Verify(
                x => x.Log(
                    LogLevel.Error,
                    It.IsAny<EventId>(),
                    It.IsAny<It.IsAnyType>(),
                    It.IsAny<Exception>(),
                    It.IsAny<Func<It.IsAnyType, Exception?, string>>()),
                Times.Once);
        }
    }
}