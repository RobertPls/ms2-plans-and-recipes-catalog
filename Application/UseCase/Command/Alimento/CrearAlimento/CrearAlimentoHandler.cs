using Shared.Core;
using MediatR;
using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Alimento.CrearAlimento
{
    public class CrearAlimentoHandler : IRequestHandler<CrearAlimentoCommand, Guid>
    {
        private readonly IAlimentoRepository _repository;
        private readonly IAlimentoFactory _factory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CrearAlimentoHandler> _logger;

        public CrearAlimentoHandler(
            IAlimentoRepository repository,
            IAlimentoFactory factory,
            IUnitOfWork unitOfWork,
            ILogger<CrearAlimentoHandler> logger)
        {
            _repository = repository;
            _factory = factory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(CrearAlimentoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var info = new InfoNutricional(
                    request.Gramos, request.Calorias, request.Proteinas,
                    request.Carbohidratos, request.Grasas
                );

                var alimento = _factory.Create(request.Nombre, request.Categoria, info);

                await _repository.CreateAsync(alimento);
                await _unitOfWork.Commit();

                return alimento.Id.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear alimento");
                throw;
            }
        }
    }
}
