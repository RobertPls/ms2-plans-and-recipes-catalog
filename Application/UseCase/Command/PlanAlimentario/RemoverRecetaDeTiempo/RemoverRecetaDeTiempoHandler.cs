using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.PlanAlimentario;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.RemoverRecetaDeTiempo
{
    public class RemoverRecetaDeTiempoHandler : IRequestHandler<RemoverRecetaDeTiempoCommand, Result>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoverRecetaDeTiempoHandler> _logger;

        public RemoverRecetaDeTiempoHandler(
            IPlanAlimentarioRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<RemoverRecetaDeTiempoHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoverRecetaDeTiempoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(request.PlanId);
                if (plan == null)
                    return Result.Fail("Plan alimentario no encontrado");

                plan.RemoverRecetaDeTiempo(request.TiempoComidaId, request.RecetaId);

                await _unitOfWork.Commit();
                return Result.Ok("Receta removida del tiempo de comida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover receta del tiempo de comida");
                return Result.Fail(ex.Message);
            }
        }
    }
}
