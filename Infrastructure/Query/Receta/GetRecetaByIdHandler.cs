using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Application.Utils;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.Repository.Receta;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Receta
{
    public class GetRecetaByIdHandler : IRequestHandler<GetRecetaByIdQuery, Result<RecetaDto>>
    {
        private readonly IRecetaRepository _repository;
        private readonly IAlimentoRepository _alimentoRepository;
        private readonly ILogger<GetRecetaByIdHandler> _logger;

        public GetRecetaByIdHandler(
            IRecetaRepository repository,
            IAlimentoRepository alimentoRepository,
            ILogger<GetRecetaByIdHandler> logger)
        {
            _repository = repository;
            _alimentoRepository = alimentoRepository;
            _logger = logger;
        }

        public async Task<Result<RecetaDto>> Handle(GetRecetaByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _repository.FindByIdAsync(request.Id);
                if (receta == null)
                    return Result.Fail<RecetaDto>("Receta no encontrada");

                var dto = new RecetaDto
                {
                    Id = receta.Id,
                    Nombre = receta.Nombre,
                    Instrucciones = receta.Instrucciones
                };

                foreach (var ing in receta.Ingredientes)
                {
                    var alimento = await _alimentoRepository.FindByIdAsync(ing.AlimentoId);
                    dto.Ingredientes.Add(new IngredienteDto
                    {
                        AlimentoId = ing.AlimentoId,
                        NombreAlimento = alimento?.Nombre ?? "Unknown",
                        Cantidad = ing.Porcion.Cantidad
                    });
                }

                return Result.Ok<RecetaDto>(dto, "Receta obtenida exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener receta con id: {RecetaId}", request.Id);
                return Result.Fail<RecetaDto>(ex.Message);
            }
        }
    }
}
