using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Application.Utils;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Receta
{
    public class GetInfoNutricionalRecetaHandler : IRequestHandler<GetInfoNutricionalRecetaQuery, Result<InfoNutricionalDto>>
    {
        private readonly IRecetaRepository _recetaRepository;
        private readonly IAlimentoRepository _alimentoRepository;
        private readonly ILogger<GetInfoNutricionalRecetaHandler> _logger;

        public GetInfoNutricionalRecetaHandler(
            IRecetaRepository recetaRepository,
            IAlimentoRepository alimentoRepository,
            ILogger<GetInfoNutricionalRecetaHandler> logger)
        {
            _recetaRepository = recetaRepository;
            _alimentoRepository = alimentoRepository;
            _logger = logger;
        }

        public async Task<Result<InfoNutricionalDto>> Handle(GetInfoNutricionalRecetaQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _recetaRepository.FindByIdAsync(RecetaId.From(request.RecetaId));
                if (receta == null)
                    return Result.Fail<InfoNutricionalDto>("Receta no encontrada");

                var info = receta.CalcularInfoNutricionalTotal(id =>
                {
                    var task = _alimentoRepository.FindByIdAsync(id);
                    task.Wait();
                    return task.Result!;
                });

                return Result.Ok<InfoNutricionalDto>(new InfoNutricionalDto
                {
                    Gramos = info.Gramos,
                    Calorias = info.Calorias,
                    Proteinas = info.Proteinas,
                    Carbohidratos = info.Carbohidratos,
                    Grasas = info.Grasas
                }, "Información nutricional obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al calcular info nutricional de receta {RecetaId}", request.RecetaId);
                return Result.Fail<InfoNutricionalDto>(ex.Message);
            }
        }
    }
}
