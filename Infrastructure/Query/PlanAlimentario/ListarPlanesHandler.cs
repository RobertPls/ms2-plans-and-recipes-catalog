using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.Infrastructure.EntityFramework.Context;
using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.PlanAlimentario
{
    public class ListarPlanesHandler : IRequestHandler<ListarPlanesQuery, Result<IEnumerable<PlanAlimentarioDto>>>
    {
        private readonly ReadDbContext _dbContext;
        private readonly ILogger<ListarPlanesHandler> _logger;

        public ListarPlanesHandler(ReadDbContext dbContext, ILogger<ListarPlanesHandler> logger)
        {
            _dbContext = dbContext;
            _logger = logger;
        }

        public async Task<Result<IEnumerable<PlanAlimentarioDto>>> Handle(ListarPlanesQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var planes = await _dbContext.PlanAlimentario.ToListAsync(cancellationToken);
                var dtoItems = planes.Select(p => new PlanAlimentarioDto
                {
                    Id = p.Id,
                    Nombre = p.Nombre,
                    DuracionTipo = p.DuracionTipo,
                    DiasTotal = p.DuracionTipo == "QUINCENAL" ? 15 : 30
                }).ToList();

                return Result.Ok<IEnumerable<PlanAlimentarioDto>>(dtoItems, "Listado de planes obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar planes");
                return Result.Fail<IEnumerable<PlanAlimentarioDto>>(ex.Message);
            }
        }
    }
}
