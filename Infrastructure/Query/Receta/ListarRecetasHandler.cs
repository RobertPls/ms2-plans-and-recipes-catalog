using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Infrastructure.EntityFramework.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Receta
{
    public class ListarRecetasHandler : IRequestHandler<ListarRecetasQuery, Result<PagedList<RecetaDto>>>
    {
        private readonly ReadDbContext _dbContext;
        private readonly ILogger<ListarRecetasHandler> _logger;

        public ListarRecetasHandler(ReadDbContext dbContext, ILogger<ListarRecetasHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<PagedList<RecetaDto>>> Handle(ListarRecetasQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var recetas = await _dbContext.Receta.Include(r => r.Ingredientes).ToListAsync(cancellationToken);
                var dtoItems = recetas.Select(r =>
                {
                    var dto = new RecetaDto
                    {
                        Id = r.Id,
                        Nombre = r.Nombre,
                        Instrucciones = r.Instrucciones
                    };
                    foreach (var ing in r.Ingredientes)
                    {
                        dto.Ingredientes.Add(new IngredienteDto
                        {
                            AlimentoId = ing.AlimentoId,
                            Cantidad = ing.PorcionCantidad
                        });
                    }
                    return dto;
                }).AsQueryable();

                return Result.Ok<PagedList<RecetaDto>>(
                    PagedList<RecetaDto>.Create(dtoItems, request.Page, request.PageSize),
                    "Listado de recetas obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar recetas");
                return Result.Fail<PagedList<RecetaDto>>(ex.Message);
            }
        }
    }
}
