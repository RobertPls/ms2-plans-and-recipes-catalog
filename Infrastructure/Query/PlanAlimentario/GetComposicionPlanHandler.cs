using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.Application.Utils;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.PlanAlimentario
{
    public class GetComposicionPlanHandler : IRequestHandler<GetComposicionPlanQuery, Result<ComposicionPlanDto>>
    {
        private readonly IPlanAlimentarioRepository _planRepository;
        private readonly IRecetaRepository _recetaRepository;
        private readonly IAlimentoRepository _alimentoRepository;
        private readonly ILogger<GetComposicionPlanHandler> _logger;

        public GetComposicionPlanHandler(
            IPlanAlimentarioRepository planRepository,
            IRecetaRepository recetaRepository,
            IAlimentoRepository alimentoRepository,
            ILogger<GetComposicionPlanHandler> logger)
        {
            _planRepository = planRepository;
            _recetaRepository = recetaRepository;
            _alimentoRepository = alimentoRepository;
            _logger = logger;
        }

        public async Task<Result<ComposicionPlanDto>> Handle(GetComposicionPlanQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var plan = await _planRepository.FindByIdAsync(PlanId.From(request.PlanId));
                if (plan == null)
                    return Result.Fail<ComposicionPlanDto>("Plan alimentario no encontrado");

                var dto = new ComposicionPlanDto
                {
                    PlanId = plan.Id.Value,
                    Nombre = plan.Nombre
                };

                foreach (var dia in plan.DiasDelPlan.OrderBy(d => d.NumeroDia))
                {
                    var diaDto = new ComposicionDiaDto { NumeroDia = dia.NumeroDia };

                    foreach (var tiempo in dia.TiemposDeComida.OrderBy(t => t.Orden))
                    {
                        var tiempoDto = new ComposicionTiempoDto
                        {
                            TiempoDeComidaId = tiempo.Id,
                            Nombre = tiempo.Nombre,
                            Orden = tiempo.Orden
                        };

                        foreach (var asig in tiempo.RecetasAsignadas)
                        {
                            var receta = await _recetaRepository.FindByIdAsync(asig.RecetaId);
                            if (receta == null) continue;

                            var recetaDto = new ComposicionRecetaDto
                            {
                                RecetaId = receta.Id.Value,
                                NombreReceta = receta.Nombre,
                                Racion = new RacionDto { Cantidad = asig.Racion.Cantidad }
                            };

                            foreach (var ing in receta.Ingredientes)
                            {
                                var alimento = await _alimentoRepository.FindByIdAsync(ing.AlimentoId);
                                    recetaDto.Ingredientes.Add(new ComposicionIngredienteDto
                                    {
                                        AlimentoId = ing.AlimentoId.Value,
                                        NombreAlimento = alimento?.Nombre ?? "Unknown",
                                        Porcion = new PorcionDto
                                        {
                                            Cantidad = ing.Porcion.Cantidad
                                        }
                                    });
                            }

                            tiempoDto.Recetas.Add(recetaDto);
                        }

                        diaDto.TiemposDeComida.Add(tiempoDto);
                    }

                    dto.Dias.Add(diaDto);
                }

                return Result.Ok<ComposicionPlanDto>(dto, "Composición del plan obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener composicion del plan {PlanId}", request.PlanId);
                return Result.Fail<ComposicionPlanDto>(ex.Message);
            }
        }
    }
}
