using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.Domain.Repository.PlanAlimentario;
using Catalog.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.PlanAlimentario
{
    public class ListarPlanesHandler : IRequestHandler<ListarPlanesQuery, PagedList<PlanAlimentarioDto>>
    {
        private readonly IPlanAlimentarioRepository _repository;
        private readonly ILogger<ListarPlanesHandler> _logger;

        public ListarPlanesHandler(IPlanAlimentarioRepository repository, ILogger<ListarPlanesHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedList<PlanAlimentarioDto>> Handle(ListarPlanesQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var planes = await _repository.FindAllAsync();
                var dtoItems = planes.Select(p => new PlanAlimentarioDto
                {
                    Id = p.Id.Value,
                    Nombre = p.Nombre,
                    DuracionTipo = p.Duracion.Tipo.ToString(),
                    DiasTotal = p.Duracion.Dias(),
                    FechaInicio = p.FechaInicio
                }).AsQueryable();

                return PagedList<PlanAlimentarioDto>.Create(dtoItems, request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al listar planes");
                return PagedList<PlanAlimentarioDto>.Create(new List<PlanAlimentarioDto>().AsQueryable(), 1, 10);
            }
        }
    }
}
