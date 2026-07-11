using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Factory.Alimento;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Alimento.CrearAlimento
{
    public class CrearAlimentoHandler : IRequestHandler<CrearAlimentoCommand, Result<Guid>>
    {
        private readonly IAlimentoRepository _repository;
        private readonly IAlimentoFactory _factory;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<CrearAlimentoHandler> _logger;

        public CrearAlimentoHandler(
            IAlimentoRepository repository,
            IAlimentoFactory factory,
            IUnitOfWork unitOfWork,
            ILogger<CrearAlimentoHandler> logger)
        {
            _repository = repository;
            _factory = factory;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result<Guid>> Handle(CrearAlimentoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                if (!Enum.IsDefined(typeof(UnidadMedida), request.UnidadMedida))
                {
                    var validos = string.Join(", ", Enum.GetValues<UnidadMedida>().Select(v => $"{(int)v}={v}"));
                    return Result.Fail<Guid>($"Unidad de medida inválida: {request.UnidadMedida}. Valores válidos: {validos}");
                }

                var nombre = request.Nombre.ToUpperInvariant();
                var categoria = request.Categoria.ToUpperInvariant();
                var unidadMedida = (UnidadMedida)request.UnidadMedida;
                var info = new InfoNutricional(
                    request.Cantidad, request.Calorias, request.Proteinas,
                    request.Carbohidratos, request.Grasas
                );

                var alimento = _factory.Create(nombre, categoria, unidadMedida, info);

                await _repository.CreateAsync(alimento);
                await _unitOfWork.Commit();

                return Result<Guid>.Ok(alimento.Id.Value, "Alimento creado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al crear alimento");
                return Result.Fail<Guid>(ex.Message);
            }
        }
    }
}
