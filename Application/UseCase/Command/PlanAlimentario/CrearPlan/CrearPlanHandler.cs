using Catalog.Shared.Core;
using Catalog.Domain;
using Catalog.Domain.Factory.PlanAlimentario;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan
{
    public class CrearPlanHandler : IRequestHandler<CrearPlanCommand, Guid>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly IPlanAlimentarioFactory _factory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CrearPlanHandler> _logger;

        public CrearPlanHandler(
            IPlanAlimentarioRepository repository,
            IPlanAlimentarioFactory factory,
            IUnitOfWork unitOfWork,
            ILogger<CrearPlanHandler> logger)
        {
            _repository = repository;
            _factory = factory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Guid> Handle(CrearPlanCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var duracion = new DuracionPlan(
                    Enum.Parse<TipoDuracion>(request.DuracionTipo)
                );

                var plan = _factory.Create(request.Nombre, duracion, request.FechaInicio);

                await _repository.CreateAsync(plan);
                await _unitOfWork.Commit();

                return plan.Id.Value;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear plan alimentario");
                throw;
            }
        }
    }
}
