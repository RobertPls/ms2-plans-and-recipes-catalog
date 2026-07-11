using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Factory.Receta;
using Catalog.Domain.Repository.Receta;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Receta.CrearReceta
{
    public class CrearRecetaHandler : IRequestHandler<CrearRecetaCommand, Result<Guid>>
    {
        private readonly IRecetaRepository _repository;
        private readonly IRecetaFactory _factory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CrearRecetaHandler> _logger;

        public CrearRecetaHandler(
            IRecetaRepository repository,
            IRecetaFactory factory,
            IUnitOfWork unitOfWork,
            ILogger<CrearRecetaHandler> logger)
        {
            _repository = repository;
            _factory = factory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CrearRecetaCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = _factory.Create(request.Nombre, request.Instrucciones);

                await _repository.CreateAsync(receta);
                await _unitOfWork.Commit();

                return Result<Guid>.Ok(receta.Id, "Receta creada exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear receta");
                return Result.Fail<Guid>(ex.Message);
            }
        }
    }
}
