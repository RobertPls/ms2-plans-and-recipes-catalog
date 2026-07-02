using Shared.Core;
using MediatR;
using Catalog.Domain.Repository.Receta;
using Catalog.Domain.ValueObjects;
using Microsoft.Extensions.Logging;

namespace Catalog.Application.UseCase.Command.Receta.AgregarIngrediente
{
    public class AgregarIngredienteHandler : IRequestHandler<AgregarIngredienteCommand, bool>
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

        public async Task<bool> Handle(AgregarIngredienteCommand request, CancellationToken cancellationToken = default)
        {
            try
            {
                var receta = await _recetaRepository.FindByIdAsync(RecetaId.From(request.RecetaId));
                if (receta == null) return false;

                receta.AgregarIngrediente(
                    AlimentoId.From(request.AlimentoId),
                    new Porcion(request.Cantidad, request.Unidad)
                );

                await _unitOfWork.Commit();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error al agregar ingrediente");
                return false;
            }
        }
    }
}
