using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Catalog.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Receta
{
    public class GetRecetaByIdHandler : IRequestHandler<GetRecetaByIdQuery, RecetaDto?>
    {
        private readonly IRecetaRepository _repository;
        private readonly ILogger<GetRecetaByIdHandler> _logger;

        public GetRecetaByIdHandler(IRecetaRepository repository, ILogger<GetRecetaByIdHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<RecetaDto?> Handle(GetRecetaByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _repository.FindByIdAsync(RecetaId.From(request.Id));
                if (receta == null) return null;

                var dto = new RecetaDto
                {
                    Id = receta.Id.Value,
                    Nombre = receta.Nombre,
                    Instrucciones = receta.Instrucciones
                };

                foreach (var ing in receta.Ingredientes)
                {
                    dto.Ingredientes.Add(new IngredienteDto
                    {
                        AlimentoId = ing.AlimentoId.Value,
                        Cantidad = ing.Porcion.Cantidad,
                        Unidad = ing.Porcion.Unidad
                    });
                }

                return dto;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener receta con id: {RecetaId}", request.Id);
                return null;
            }
        }
    }
}
