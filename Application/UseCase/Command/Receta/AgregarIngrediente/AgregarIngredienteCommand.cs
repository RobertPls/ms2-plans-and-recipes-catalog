using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.UseCase.Command.Receta.AgregarIngrediente
{
    public class AgregarIngredienteCommand : IRequest<Result>
    {
        public Guid RecetaId { get; set; }

        [MinLength(1, ErrorMessage = "Debe agregar al menos un ingrediente")]
        public List<IngredienteItem> Ingredientes { get; set; } = new();
    }

    public class IngredienteItem
    {
        public Guid AlimentoId { get; set; }
        public decimal Cantidad { get; set; }
    }
}
