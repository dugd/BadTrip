using BadTrip.Application.Features.Hotels.Commands.CreateHotel;
using BadTrip.Application.Features.Hotels.Commands.DeleteHotel;
using BadTrip.Application.Features.Hotels.Commands.UpdateHotel;
using BadTrip.Application.Features.Hotels.Queries.GetAllHotels;
using BadTrip.Application.Features.Hotels.Queries.GetHotel;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadTrip.API.Controllers
{
    [ApiController]
    [Route("api/hotels")]
    [Authorize(Roles = "Admin")]
    public class HotelsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public HotelsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateHotel([FromBody] CreateHotelCommand command, CancellationToken ct)
        {
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        [HttpGet]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetAllHotels(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllHotelsQuery(), ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetHotel(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetHotelQuery(id), ct);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateHotel(Guid id, [FromBody] UpdateHotelCommand command, CancellationToken ct)
        {
            // Ensure route ID matches command ID
            if (id != command.Id)
            {
                return BadRequest(new { Message = "Route ID does not match command ID." });
            }

            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeleteHotel(Guid id, CancellationToken ct)
        {
            await _mediator.Send(new DeleteHotelCommand(id), ct);
            return NoContent();
        }
    }
}
