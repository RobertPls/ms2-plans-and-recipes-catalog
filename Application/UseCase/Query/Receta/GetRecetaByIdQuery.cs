using Catalog.Application.Dto;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Receta
{
    public class GetRecetaByIdQuery : IRequest<RecetaDto?>
    {
        public Guid Id { get; set; }
        public GetRecetaByIdQuery() { }
        public GetRecetaByIdQuery(Guid id) => Id = id;
    }
}
