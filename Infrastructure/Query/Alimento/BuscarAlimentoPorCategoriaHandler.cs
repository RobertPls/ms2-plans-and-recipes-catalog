using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Domain.Repository.Alimento;
using Catalog.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Alimento
{
    public class BuscarAlimentoPorCategoriaHandler : IRequestHandler<BuscarAlimentoPorCategoriaQuery, PagedList<AlimentoDto>>
    {
        private readonly IAlimentoRepository _repository;
        private readonly ILogger<BuscarAlimentoPorCategoriaHandler> _logger;

        public BuscarAlimentoPorCategoriaHandler(IAlimentoRepository repository, ILogger<BuscarAlimentoPorCategoriaHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<PagedList<AlimentoDto>> Handle(BuscarAlimentoPorCategoriaQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimentos = await _repository.FindByCategoryAsync(request.Categoria);
                var dtoItems = alimentos.Select(a => new AlimentoDto
                {
                    Id = a.Id.Value,
                    Nombre = a.Nombre,
                    Categoria = a.Categoria,
                    Gramos = a.InfoNutricionalBase.Gramos,
                    Calorias = a.InfoNutricionalBase.Calorias,
                    Proteinas = a.InfoNutricionalBase.Proteinas,
                    Carbohidratos = a.InfoNutricionalBase.Carbohidratos,
                    Grasas = a.InfoNutricionalBase.Grasas
                }).AsQueryable();

                return PagedList<AlimentoDto>.Create(dtoItems, request.Page, request.PageSize);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al buscar alimentos por categoria");
                return PagedList<AlimentoDto>.Create(new List<AlimentoDto>().AsQueryable(), 1, 10);
            }
        }
    }
}
