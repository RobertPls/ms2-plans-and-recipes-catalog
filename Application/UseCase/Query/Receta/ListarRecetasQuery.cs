using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class ListarRecetasQuery : IRequest<Result<PagedList<RecetaDto>>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
