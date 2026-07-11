using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class GetInfoNutricionalRecetaQuery : IRequest<Result<InfoNutricionalDto>>
    {
        public Guid RecetaId { get; set; }
        public GetInfoNutricionalRecetaQuery() { }
        public GetInfoNutricionalRecetaQuery(Guid recetaId) => RecetaId = recetaId;
    }

    public class InfoNutricionalDto
    {
        public decimal Cantidad { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Grasas { get; set; }
    }
}
