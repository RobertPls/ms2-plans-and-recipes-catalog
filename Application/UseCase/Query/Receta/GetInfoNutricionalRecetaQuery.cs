using Catalog.Application.Dto;
using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class GetInfoNutricionalRecetaQuery : IRequest<InfoNutricionalDto?>
    {
        public Guid RecetaId { get; set; }
        public GetInfoNutricionalRecetaQuery() { }
        public GetInfoNutricionalRecetaQuery(Guid recetaId) => RecetaId = recetaId;
    }

    public class InfoNutricionalDto
    {
        public decimal Gramos { get; set; }
        public decimal Calorias { get; set; }
        public decimal Proteinas { get; set; }
        public decimal Carbohidratos { get; set; }
        public decimal Grasas { get; set; }
    }
}
