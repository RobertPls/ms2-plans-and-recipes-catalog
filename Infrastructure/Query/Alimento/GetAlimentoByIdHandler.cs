using Catalog.Application.Dto;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Application.Utils;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Shared.Core;
using MediatR;
using Microsoft.Extensions.Logging;

namespace Catalog.Infrastructure.Query.Alimento
{
    public class GetAlimentoByIdHandler : IRequestHandler<GetAlimentoByIdQuery, Result<AlimentoDto>>
    {
        private readonly IAlimentoRepository _repository;
        private readonly ILogger<GetAlimentoByIdHandler> _logger;

        public GetAlimentoByIdHandler(IAlimentoRepository repository, ILogger<GetAlimentoByIdHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<Result<AlimentoDto>> Handle(GetAlimentoByIdQuery request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimento = await _repository.FindByIdAsync(AlimentoId.From(request.Id));
                if (alimento == null)
                    return Result.Fail<AlimentoDto>("Alimento no encontrado");

                return Result.Ok<AlimentoDto>(new AlimentoDto
                {
                    Id = alimento.Id.Value,
                    Nombre = alimento.Nombre,
                    Categoria = alimento.Categoria,
                    UnidadMedida = alimento.UnidadMedida.ToString(),
                    Cantidad = alimento.InfoNutricionalBase.Cantidad,
                    Calorias = alimento.InfoNutricionalBase.Calorias,
                    Proteinas = alimento.InfoNutricionalBase.Proteinas,
                    Carbohidratos = alimento.InfoNutricionalBase.Carbohidratos,
                    Grasas = alimento.InfoNutricionalBase.Grasas
                }, "Alimento obtenido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al obtener alimento con id: {AlimentoId}", request.Id);
                return Result.Fail<AlimentoDto>(ex.Message);
            }
        }
    }
}
