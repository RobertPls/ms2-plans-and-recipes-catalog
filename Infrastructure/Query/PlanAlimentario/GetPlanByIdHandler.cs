using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.PlanAlimentario
{
    public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, PlanAlimentarioDto?>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly ILogger<GetPlanByIdHandler> _logger;

        public GetPlanByIdHandler(IPlanAlimentarioRepository repository, ILogger<GetPlanByIdHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PlanAlimentarioDto?> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(PlanId.From(request.Id));
                if (plan == null) return null;

                var dto = new PlanAlimentarioDto
                {
                    Id = plan.Id.Value,
                    Nombre = plan.Nombre,
                    DuracionTipo = plan.Duracion.Tipo.ToString(),
                    DiasTotal = plan.Duracion.Dias(),
                    FechaInicio = plan.FechaInicio
                };

                foreach (var dia in plan.DiasDelPlan)
                {
                    var diaDto = new DiaDelPlanDto { NumeroDia = dia.NumeroDia };
                    foreach (var tiempo in dia.TiemposDeComida)
                    {
                        var tDto = new TiempoDeComidaDto
                        {
                            Nombre = tiempo.Nombre,
                            Orden = tiempo.Orden
                        };
                        foreach (var asig in tiempo.RecetasAsignadas)
                        {
                            tDto.Recetas.Add(new RecetaAsignadaDto
                            {
                                RecetaId = asig.RecetaId.Value,
                                Racion = asig.Racion.Cantidad
                            });
                        }
                        diaDto.Tiempos.Add(tDto);
                    }
                    dto.Dias.Add(diaDto);
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plan con id: {PlanId}", request.Id);
                return null;
            }
        }
    }
}
