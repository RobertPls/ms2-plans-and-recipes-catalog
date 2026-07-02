using Catalog.Application.UseCase.Command.Receta.CrearReceta;
using Catalog.Application.UseCase.Command.Receta.AgregarIngrediente;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.Shared.Core;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/recetas")]
    public class RecetaController : ControllerBase
    {
        private readonly IMediator _mediator;

        public RecetaController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearRecetaCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

        [HttpPost("{recetaId:guid}/ingredientes")]
        public async Task<IActionResult> AgregarIngrediente(Guid recetaId, [FromBody] AgregarIngredienteCommand command)
        {
            command.RecetaId = recetaId;
            var result = await _mediator.Send(command);
            return result ? Ok() : BadRequest();
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRecetaByIdQuery(id));
            if (result == null) return NotFound();
            return Ok(result);
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListarRecetasQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(result);
        }

        [HttpGet("{recetaId:guid}/info-nutricional")]
        public async Task<IActionResult> GetInfoNutricional(Guid recetaId)
        {
            var result = await _mediator.Send(new GetInfoNutricionalRecetaQuery(recetaId));
            if (result == null) return NotFound();
            return Ok(result);
        }
    }
}
