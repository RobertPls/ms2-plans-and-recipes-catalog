using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.PlanAlimentario
{
    public class GetPlanByIdQuery : IRequest<Result<PlanAlimentarioDto>>
    {
        public Guid Id { get; set; }
        public GetPlanByIdQuery() { }
        public GetPlanByIdQuery(Guid id) => Id = id;
    }
}
