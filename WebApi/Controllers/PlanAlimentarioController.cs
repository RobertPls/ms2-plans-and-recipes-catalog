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
            var id = await _mediator.Send(command);
            return Ok(ApiResponse<Guid>.Ok(id, "Plan alimentario creado exitosamente"));
        }

        [HttpPost("{planId:guid}/dias/{numDia}/tiempos")]
        public async Task<IActionResult> AgregarTiempoComida(Guid planId, int numDia, [FromBody] AgregarTiempoComidaCommand command)
        {
            command.PlanId = planId;
            command.NumDia = numDia;
            var result = await _mediator.Send(command);
            return result
                ? Ok(ApiResponse.Ok("Tiempo de comida agregado exitosamente"))
                : BadRequest(ApiResponse.Fail("No se pudo agregar el tiempo de comida"));
        }

        [HttpPost("{planId:guid}/dias/{numDia}/tiempos/{tId:guid}/recetas")]
        public async Task<IActionResult> AsignarReceta(Guid planId, int numDia, Guid tId, [FromBody] AsignarRecetaATiempoCommand command)
        {
            command.PlanId = planId;
            command.NumDia = numDia;
            command.TiempoComidaId = tId;
            var result = await _mediator.Send(command);
            return result
                ? Ok(ApiResponse.Ok("Receta asignada exitosamente"))
                : BadRequest(ApiResponse.Fail("No se pudo asignar la receta"));
        }

        [HttpGet("{id:guid}")]
        public async Task<IActionResult> GetById(Guid id)
        {
            var result = await _mediator.Send(new GetPlanByIdQuery(id));
            if (result == null) return NotFound(ApiResponse.Fail("Plan alimentario no encontrado"));
            return Ok(ApiResponse.Ok(result, "Plan alimentario obtenido exitosamente"));
        }

        [HttpGet]
        public async Task<IActionResult> List([FromQuery] int page = 1, [FromQuery] int pageSize = 10)
        {
            var query = new ListarPlanesQuery { Page = page, PageSize = pageSize };
            var result = await _mediator.Send(query);
            return Ok(ApiResponse.Ok(result, "Listado de planes obtenido exitosamente"));
        }

        [HttpGet("{planId:guid}/composicion")]
        public async Task<IActionResult> GetComposicion(Guid planId)
        {
            var result = await _mediator.Send(new GetComposicionPlanQuery(planId));
            if (result == null) return NotFound(ApiResponse.Fail("Composición del plan no encontrada"));
            return Ok(ApiResponse.Ok(result, "Composición del plan obtenida exitosamente"));
        }
    }
}
