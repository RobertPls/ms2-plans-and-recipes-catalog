using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.PlanAlimentario
{
    public class ListarPlanesQuery : IRequest<Result<IEnumerable<PlanAlimentarioDto>>>
    {
    }
}
