using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Domain.Repository.Receta;
using Catalog.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Receta
{
    public class ListarRecetasHandler : IRequestHandler<ListarRecetasQuery, PagedList<RecetaDto>>
    {
        private readonly IRecetaRepository _repository;
        private readonly ILogger<ListarRecetasHandler> _logger;

        public ListarRecetasHandler(IRecetaRepository repository, ILogger<ListarRecetasHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedList<RecetaDto>> Handle(ListarRecetasQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var recetas = await _repository.FindAllAsync();
                var dtoItems = recetas.Select(r =>
                {
                    var dto = new RecetaDto
                    {
                        Id = r.Id.Value,
                        Nombre = r.Nombre,
                        Instrucciones = r.Instrucciones
                    };
                    foreach (var ing in r.Ingredientes)
                    {
                        dto.Ingredientes.Add(new IngredienteDto
                        {
                            AlimentoId = ing.AlimentoId.Value,
                            Cantidad = ing.Porcion.Cantidad,
                            Unidad = ing.Porcion.Unidad
                        });
                    }
                    return dto;
                }).AsQueryable();

                return PagedList<RecetaDto>.Create(dtoItems, request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar recetas");
                return PagedList<RecetaDto>.Create(new List<RecetaDto>().AsQueryable(), 1, 10);
            }
        }
    }
}
