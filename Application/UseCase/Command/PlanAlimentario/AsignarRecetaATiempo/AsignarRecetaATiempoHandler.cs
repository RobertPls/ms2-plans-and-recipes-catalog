using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo
{
    public class AsignarRecetaATiempoHandler : IRequestHandler<AsignarRecetaATiempoCommand, bool>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AsignarRecetaATiempoHandler> _logger;

        public AsignarRecetaATiempoHandler(
            IPlanAlimentarioRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<AsignarRecetaATiempoHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<bool> Handle(AsignarRecetaATiempoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(PlanId.From(request.PlanId));
                if (plan == null) return false;

                plan.AsignarRecetaATiempo(
                    request.NumDia,
                    request.TiempoComidaId,
                    RecetaId.From(request.RecetaId),
                    new Racion(request.RacionCantidad)
                );

                await _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar receta a tiempo de comida");
                return false;
            }
        }
    }
}
