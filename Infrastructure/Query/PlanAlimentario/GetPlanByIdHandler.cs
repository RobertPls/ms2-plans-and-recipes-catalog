using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.Application.Utils;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.Repository.Receta;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.PlanAlimentario
{
    public class GetPlanByIdHandler : IRequestHandler<GetPlanByIdQuery, Result<PlanAlimentarioDto>>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly IRecetaRepository _recetaRepository;
        private readonly ILogger<GetPlanByIdHandler> _logger;

        public GetPlanByIdHandler(IPlanAlimentarioRepository repository, IRecetaRepository recetaRepository, ILogger<GetPlanByIdHandler> logger)
        {
            _repository = repository;
            _recetaRepository = recetaRepository;
            _logger = logger;
        }

        public async Task<Result<PlanAlimentarioDto>> Handle(GetPlanByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _repository.FindByIdAsync(request.Id);
                if (plan == null)
                    return Result.Fail<PlanAlimentarioDto>("Plan alimentario no encontrado");

                var dto = new PlanAlimentarioDto
                {
                    Id = plan.Id,
                    Nombre = plan.Nombre,
                    DuracionTipo = plan.Duracion.Tipo.ToString(),
                    DiasTotal = plan.Duracion.Dias(),
                    ComidasPorDia = plan.ComidasPorDia
                };

                foreach (var dia in plan.DiasDelPlan.OrderBy(d => d.NumeroDia))
                {
                    var diaDto = new DiaDelPlanDto { NumeroDia = dia.NumeroDia };
                    foreach (var tiempo in dia.TiemposDeComida.OrderBy(t => t.Orden))
                    {
                        var tDto = new TiempoDeComidaDto
                        {
                            Nombre = tiempo.Nombre,
                            Orden = tiempo.Orden
                        };
                        foreach (var asig in tiempo.RecetasAsignadas)
                        {
                            var receta = await _recetaRepository.FindByIdAsync(asig.RecetaId);
                            tDto.Recetas.Add(new RecetaAsignadaDto
                            {
                                RecetaId = asig.RecetaId,
                                NombreReceta = receta?.Nombre ?? "Unknown",
                                Racion = asig.Racion.Cantidad
                            });
                        }
                        diaDto.Tiempos.Add(tDto);
                    }
                    dto.Dias.Add(diaDto);
                }

                return Result.Ok<PlanAlimentarioDto>(dto, "Plan alimentario obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener plan con id: {PlanId}", request.Id);
                return Result.Fail<PlanAlimentarioDto>(ex.Message);
            }
        }
    }
}
