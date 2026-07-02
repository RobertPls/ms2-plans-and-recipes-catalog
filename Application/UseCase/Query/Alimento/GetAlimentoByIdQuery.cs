using Catalog.Application.Dto;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Alimento
{
    public class GetAlimentoByIdQuery : IRequest<AlimentoDto?>
    {
        public Guid Id { get; set; }
        public GetAlimentoByIdQuery() { }
        public GetAlimentoByIdQuery(Guid id) => Id = id;
    }
}
