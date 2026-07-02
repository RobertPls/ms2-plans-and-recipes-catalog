using Catalog.Shared.Core;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Alimento.ActualizarInfoNutricional
{
    public class ActualizarInfoNutricionalHandler : IRequestHandler<ActualizarInfoNutricionalCommand, bool>
    {
        private readonly IAlimentoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActualizarInfoNutricionalHandler> _logger;

        public ActualizarInfoNutricionalHandler(
            IAlimentoRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<ActualizarInfoNutricionalHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(ActualizarInfoNutricionalCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimento = await _repository.FindByIdAsync(AlimentoId.From(request.AlimentoId));
                if (alimento == null) return false;

                var nuevaInfo = new InfoNutricional(
                    request.Gramos, request.Calorias, request.Proteinas,
                    request.Carbohidratos, request.Grasas
                );

                alimento.ActualizarInfoNutricional(nuevaInfo);

                await _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar info nutricional");
                return false;
            }
        }
    }
}
