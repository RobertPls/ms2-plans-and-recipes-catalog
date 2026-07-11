using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Command.Receta.CrearReceta;
using Catalog.Application.UseCase.Command.Receta.AgregarIngrediente;
using Catalog.Application.UseCase.Query.Receta;
using Catalog.WebApi.Utils;
using Shared.Core;
using MediatR;
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
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse<Guid>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse<Guid>.Fail(result.Message, result.Errors));
        }

        [HttpPost("{recetaId:guid}/ingredientes")]
        public async Task<IActionResult> AgregarIngrediente(Guid recetaId, [FromBody] AgregarIngredienteCommand command)
        {
            command.RecetaId = recetaId;
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse.Ok(result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetRecetaByIdQuery(id));
            if (result.IsSuccess)
                return Ok(ApiResponse<RecetaDto>.Ok(result.Value, result.Message));
            return NotFound(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListarRecetasQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(ApiResponse<PagedList<RecetaDto>>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet("{recetaId:guid}/info-nutricional")]
        public async Task<IActionResult> GetInfoNutricional(Guid recetaId)
        {
            var result = await _mediator.Send(new GetInfoNutricionalRecetaQuery(recetaId));
            if (result.IsSuccess)
                return Ok(ApiResponse<InfoNutricionalDto>.Ok(result.Value, result.Message));
            return NotFound(ApiResponse.Fail(result.Message, result.Errors));
        }
    }
}
