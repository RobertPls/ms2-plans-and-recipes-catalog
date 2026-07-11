using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Domain.ValueObjects;
using Catalog.Infrastructure.EntityFramework.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Alimento
{
    public class ListarAlimentosHandler : IRequestHandler<ListarAlimentosQuery, Result<PagedList<AlimentoDto>>>
    {
        private readonly ReadDbContext _dbContext;
        private readonly ILogger<ListarAlimentosHandler> _logger;

        public ListarAlimentosHandler(ReadDbContext dbContext, ILogger<ListarAlimentosHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<PagedList<AlimentoDto>>> Handle(ListarAlimentosQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimentos = await _dbContext.Alimento.ToListAsync(cancellationToken);
                var dtoItems = alimentos.Select(a => new AlimentoDto
                {
                    Id = a.Id,
                    Nombre = a.Nombre,
                    Categoria = a.Categoria,
                    UnidadMedida = ((UnidadMedida)a.UnidadMedida).ToString(),
                    Cantidad = a.Cantidad,
                    Calorias = a.Calorias,
                    Proteinas = a.Proteinas,
                    Carbohidratos = a.Carbohidratos,
                    Grasas = a.Grasas
                }).AsQueryable();

                return Result.Ok<PagedList<AlimentoDto>>(
                    PagedList<AlimentoDto>.Create(dtoItems, request.Page, request.PageSize),
                    "Listado de alimentos obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar alimentos");
                return Result.Fail<PagedList<AlimentoDto>>(ex.Message);
            }
        }
    }
}
