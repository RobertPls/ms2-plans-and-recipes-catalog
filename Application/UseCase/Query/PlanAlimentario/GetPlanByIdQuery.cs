using Catalog.Application.Dto;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.PlanAlimentario
{
    public class GetPlanByIdQuery : IRequest<PlanAlimentarioDto?>
    {
        public Guid Id { get; set; }
        public GetPlanByIdQuery() { }
        public GetPlanByIdQuery(Guid id) => Id = id;
    }
}
