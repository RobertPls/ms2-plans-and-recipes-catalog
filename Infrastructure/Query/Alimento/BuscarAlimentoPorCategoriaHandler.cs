using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Infrastructure.EntityFramework.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Alimento
{
    public class BuscarAlimentoPorCategoriaHandler : IRequestHandler<BuscarAlimentoPorCategoriaQuery, Result<PagedList<AlimentoDto>>>
    {
        private readonly ReadDbContext _dbContext;
        private readonly ILogger<BuscarAlimentoPorCategoriaHandler> _logger;

        public BuscarAlimentoPorCategoriaHandler(ReadDbContext dbContext, ILogger<BuscarAlimentoPorCategoriaHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<PagedList<AlimentoDto>>> Handle(BuscarAlimentoPorCategoriaQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimentos = await _dbContext.Alimento
                    .Where(a => a.Categoria == request.Categoria)
                    .ToListAsync(cancellationToken);
                var dtoItems = alimentos.Select(a => new AlimentoDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Categoria = a.Categoria,
                    Cantidad = a.Cantidad,
                    Calorias = a.Calorias,
                    Proteinas = a.Proteinas,
                    Carbohidratos = a.Carbohidratos,
                    Grasas = a.Grasas
                }).AsQueryable();

                return Result.Ok<PagedList<AlimentoDto>>(
                    PagedList<AlimentoDto>.Create(dtoItems, request.Page, request.PageSize),
                    "Búsqueda por categoría exitosa");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar alimentos por categoria");
                return Result.Fail<PagedList<AlimentoDto>>(ex.Message);
            }
        }
    }
}
