using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Receta.AgregarIngrediente
{
    public class AgregarIngredienteHandler : IRequestHandler<AgregarIngredienteCommand, Result>
    {
        private readonly IRecetaRepository _recetaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<AgregarIngredienteHandler> _logger;

        public AgregarIngredienteHandler(
            IRecetaRepository recetaRepository,
            IUnitOfWork unitOfWork,
            ILogger<AgregarIngredienteHandler> logger)
        {
            _recetaRepository = recetaRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(AgregarIngredienteCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _recetaRepository.FindByIdAsync(request.RecetaId);
                if (receta == null)
                    return Result.Fail("Receta no encontrada");

                foreach (var item in request.Ingredientes)
                {
                    receta.AgregarIngrediente(
                        item.AlimentoId,
                        new Porcion(item.Cantidad)
                    );
                }

                await _unitOfWork.Commit();
                return Result.Ok($"{request.Ingredientes.Count} ingrediente(s) agregado(s) exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ingrediente");
                return Result.Fail(ex.Message);
            }
        }
    }
}
