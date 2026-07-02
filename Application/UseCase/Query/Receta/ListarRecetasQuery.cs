using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Shared.Core;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class ListarRecetasQuery : IRequest<PagedList<RecetaDto>>
    {
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
    }
}
