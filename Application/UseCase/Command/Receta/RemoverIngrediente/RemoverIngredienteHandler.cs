using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Receta.RemoverIngrediente
{
    public class RemoverIngredienteHandler : IRequestHandler<RemoverIngredienteCommand, Result>
    {
        private readonly IRecetaRepository _recetaRepository;
        private readonly IUnitOfWork _unitOfWork;
        private readonly ILogger<RemoverIngredienteHandler> _logger;

        public RemoverIngredienteHandler(
            IRecetaRepository recetaRepository,
            IUnitOfWork unitOfWork,
            ILogger<RemoverIngredienteHandler> logger)
        {
            _recetaRepository = recetaRepository;
            _unitOfWork = unitOfWork;
            _logger = logger;
        }

        public async Task<Result> Handle(RemoverIngredienteCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _recetaRepository.FindByIdAsync(RecetaId.From(request.RecetaId));
                if (receta == null)
                    return Result.Fail("Receta no encontrada");

                receta.RemoverIngrediente(AlimentoId.From(request.AlimentoId));

                await _unitOfWork.Commit();
                return Result.Ok("Ingrediente removido exitosamente");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al remover ingrediente");
                return Result.Fail(ex.Message);
            }
        }
    }
}
