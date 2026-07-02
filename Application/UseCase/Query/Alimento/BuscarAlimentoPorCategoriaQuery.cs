using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Shared.Core;
using MediatR;

namespace Catalog.Application.UseCase.Query.Alimento
{
    public class BuscarAlimentoPorCategoriaQuery : IRequest<PagedList<AlimentoDto>>
    {
        public string Categoria { get; set; } = null!;
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 10;
        public BuscarAlimentoPorCategoriaQuery() { }
        public BuscarAlimentoPorCategoriaQuery(string categoria) => Categoria = categoria;
    }
}
