using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Catalog.Shared.Core;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Alimento
{
    public class GetAlimentoByIdHandler : IRequestHandler<GetAlimentoByIdQuery, AlimentoDto?>
    {
        private readonly IAlimentoRepository _repository;
        private readonly ILogger<GetAlimentoByIdHandler> _logger;

        public GetAlimentoByIdHandler(IAlimentoRepository repository, ILogger<GetAlimentoByIdHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<AlimentoDto?> Handle(GetAlimentoByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimento = await _repository.FindByIdAsync(AlimentoId.From(request.Id));
                if (alimento == null) return null;

                return new AlimentoDto
                {
                    Id = alimento.Id.Value,
                    Nombre = alimento.Nombre,
                    Categoria = alimento.Categoria,
                    Gramos = alimento.InfoNutricionalBase.Gramos,
                    Calorias = alimento.InfoNutricionalBase.Calorias,
                    Proteinas = alimento.InfoNutricionalBase.Proteinas,
                    Carbohidratos = alimento.InfoNutricionalBase.Carbohidratos,
                    Grasas = alimento.InfoNutricionalBase.Grasas
                };
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alimento con id: {AlimentoId}", request.Id);
                return null;
            }
        }
    }
}
