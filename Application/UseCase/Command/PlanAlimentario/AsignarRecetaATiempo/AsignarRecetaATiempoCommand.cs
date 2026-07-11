using Catalog.Application.Utils;
using Shared.Core;
using MediatR;
using System.ComponentModel.DataAnnotations;

namespace Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo
{
    public class AsignarRecetaATiempoCommand : IRequest<Result>
    {
        public Guid PlanId { get; set; }
        public Guid TiempoComidaId { get; set; }

        [MinLength(1, ErrorMessage = "Debe asignar al menos una receta")]
        public List<RecetaItem> Recetas { get; set; } = new();
    }

    public class RecetaItem
    {
        public Guid RecetaId { get; set; }
        public int RacionCantidad { get; set; }
    }
}
