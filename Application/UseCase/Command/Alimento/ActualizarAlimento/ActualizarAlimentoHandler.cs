using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.Alimento;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Alimento.ActualizarAlimento
{
    public class ActualizarAlimentoHandler : IRequestHandler<ActualizarAlimentoCommand, Result>
    {
        private readonly IAlimentoRepository _repository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<ActualizarAlimentoHandler> _logger;

        public ActualizarAlimentoHandler(
            IAlimentoRepository repository,
            IUnitOfWork unitOfWork,
            ILogger<ActualizarAlimentoHandler> logger)
        {
            _repository = repository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(ActualizarAlimentoCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var alimento = await _repository.FindByIdAsync(AlimentoId.From(request.AlimentoId));
                if (alimento == null)
                    return Result.Fail("Alimento no encontrado");

                var nombre = request.Nombre.ToUpperInvariant();
                var categoria = request.Categoria.ToUpperInvariant();
                var unidadMedida = (UnidadMedida)request.UnidadMedida;
                var info = new InfoNutricional(
                    request.Cantidad, request.Calorias, request.Proteinas,
                    request.Carbohidratos, request.Grasas
                );

                alimento.Actualizar(nombre, categoria, unidadMedida, info);

                await _unitOfWork.Commit();
                return Result.Ok("Alimento actualizado exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al actualizar alimento");
                return Result.Fail(ex.Message);
            }
        }
    }
}
