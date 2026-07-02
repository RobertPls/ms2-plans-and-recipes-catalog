using Catalog.Application.UseCase.Command.Alimento.CrearAlimento;
using Catalog.Application.UseCase.Command.Alimento.ActualizarInfoNutricional;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.Shared.Core;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/alimentos")]
    public class AlimentoController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AlimentoController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearAlimentoCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetAlimentoByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListarAlimentosQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> GetByCategoria(string categoria, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new BuscarAlimentoPorCategoriaQuery(categoria) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpPut("{alimentoId:guid}/info-nutricional")]
        public async Task<IActionResult> ActualizarInfoNutricional(Guid alimentoId, [FromBody] ActualizarInfoNutricionalCommand command)
        {
            command.AlimentoId = alimentoId;
            var result = await _mediator.Send(command);
            return result ? Ok() : NotFound();
        }
    }
}
