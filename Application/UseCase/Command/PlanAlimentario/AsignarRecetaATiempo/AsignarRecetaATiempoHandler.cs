using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo
{
    public class AsignarRecetaATiempoHandler : IRequestHandler<AsignarRecetaATiempoCommand, Result>
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

        public async Task<Result> Handle(AsignarRecetaATiempoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(PlanId.From(request.PlanId));
                if (plan == null)
                    return Result.Fail("Plan alimentario no encontrado");

                foreach (var item in request.Recetas)
                {
                    plan.AsignarRecetaATiempo(
                        request.TiempoComidaId,
                        RecetaId.From(item.RecetaId),
                        new Racion(item.RacionCantidad)
                    );
                }

                await _unitOfWork.Commit();
                return Result.Ok($"{request.Recetas.Count} receta(s) asignada(s) exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al asignar receta a tiempo de comida");
                return Result.Fail(ex.Message);
            }
        }
    }
}
