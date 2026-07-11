using Catalog.Application.Dto;
using Catalog.Application.Utils;
using Catalog.Application.UseCase.Command.PlanAlimentario.CrearPlan;
using Catalog.Application.UseCase.Command.PlanAlimentario.AgregarTiempoComida;
using Catalog.Application.UseCase.Command.PlanAlimentario.AsignarRecetaATiempo;
using Catalog.Application.UseCase.Query.PlanAlimentario;
using Catalog.WebApi.Utils;
using Shared.Core;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace Catalog.WebApi.Controllers
{
    [ApiController]
    [Route("api/v1/planes")]
    public class PlanAlimentarioController : ControllerBase
    {
        private readonly IMediator _mediator;

        public PlanAlimentarioController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        public async Task<IActionResult> Create([FromBody] CrearPlanCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse<Guid>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse<Guid>.Fail(result.Message, result.Errors));
        }

        [HttpPost("tiempos-comida")]
        public async Task<IActionResult> AgregarTiempoComida([FromBody] AgregarTiempoComidaCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse.Ok(result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpPost("asignar-recetas")]
        public async Task<IActionResult> AsignarReceta([FromBody] AsignarRecetaATiempoCommand command)
        {
            var result = await _mediator.Send(command);
            if (result.IsSuccess)
                return Ok(ApiResponse.Ok(result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet("{planId:guid}")]
        public async Task<IActionResult> GetComposicion(Guid planId)
        {
            var result = await _mediator.Send(new GetComposicionPlanQuery(planId));
            if (result.IsSuccess)
                return Ok(ApiResponse<ComposicionPlanDto>.Ok(result.Value, result.Message));
            return NotFound(ApiResponse.Fail(result.Message, result.Errors));
        }

        [HttpGet]
        public async Task<IActionResult> List()
        {
            var result = await _mediator.Send(new ListarPlanesQuery());
            if (result.IsSuccess)
                return Ok(ApiResponse<IEnumerable<PlanAlimentarioDto>>.Ok(result.Value, result.Message));
            return BadRequest(ApiResponse.Fail(result.Message, result.Errors));
        }
    }
}
