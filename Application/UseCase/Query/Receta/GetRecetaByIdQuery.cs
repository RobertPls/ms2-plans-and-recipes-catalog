using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class GetRecetaByIdQuery : IRequest<Result<RecetaDto>>
    {
        public Guid Id { get; set; }
        public GetRecetaByIdQuery() { }
        public GetRecetaByIdQuery(Guid id) => Id = id;
    }
}
