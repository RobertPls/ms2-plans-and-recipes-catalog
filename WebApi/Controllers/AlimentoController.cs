using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Command.Alimento.CrearAlimento;
using Catalog.Application.UseCase.Command.Alimento.ActualizarInfoNutricional;
using Catalog.Application.UseCase.Query.Alimento;
using Catalog.WebApi.Utils;
using Shared.Core;
using MediatR;
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
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse<Guid>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse<Guid>.Fail(result.Message, result.Errors));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetAlimentoByIdQuery(id));
            if (result.IsSuccess)
                return Ok(ApiResponse<AlimentoDto>.Ok(result.Value, result.Message));
            return NotFound(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListarAlimentosQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(ApiResponse<PagedList<AlimentoDto>>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet("categoria/{categoria}")]
        public async Task<IActionResult> GetByCategoria(string categoria, [FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new BuscarAlimentoPorCategoriaQuery(categoria) { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            if (result.IsSuccess)
                return Ok(ApiResponse<PagedList<AlimentoDto>>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpPut("{alimentoId:guid}/info-nutricional")]
        public async Task<IActionResult> ActualizarInfoNutricional(Guid alimentoId, [FromBody] ActualizarInfoNutricionalCommand command)
        {
            command.AlimentoId = alimentoId;
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse.Ok(result.Message));
            return NotFound(ApiResponse.Fail(result.Message, result.Errors));
        }
    }
}
