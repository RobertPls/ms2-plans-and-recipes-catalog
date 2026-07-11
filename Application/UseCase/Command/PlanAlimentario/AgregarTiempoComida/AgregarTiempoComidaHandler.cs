using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AgregarTiempoComida
{
    public class AgregarTiempoComidaHandler : IRequestHandler<AgregarTiempoComidaCommand, Result>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AgregarTiempoComidaHandler> _logger;

        public AgregarTiempoComidaHandler(
            IPlanAlimentarioRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<AgregarTiempoComidaHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(AgregarTiempoComidaCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(PlanId.From(request.PlanId));
                if (plan == null)
                    return Result.Fail("Plan alimentario no encontrado");

                plan.AgregarTiempoDeComidaADia(request.NumDia, request.Nombre, request.Orden);

                await _unitOfWork.Commit();
                return Result.Ok("Tiempo de comida agregado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar tiempo de comida");
                return Result.Fail(ex.Message);
            }
        }
    }
}
