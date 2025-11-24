using BadTrip.Application.Features.Bookings.Commands.CancelBooking;
using BadTrip.Application.Features.Bookings.Commands.ConfirmBooking;
using BadTrip.Application.Features.Bookings.Commands.CreateBooking;
using BadTrip.Application.Features.Bookings.Commands.PayBooking;
using BadTrip.Application.Features.Bookings.Queries.GetBooking;
using BadTrip.Application.Features.Bookings.Queries.GetMyBookings;
using BadTrip.Application.Features.Bookings.Queries.GetTourBookings;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;

namespace BadTrip.API.Controllers
{
    [ApiController]
    [Route("api/bookings")]
    [Authorize]
    public class BookingsController : ControllerBase
    {
        private readonly IMediator _mediator;

        public BookingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        private Guid GetUserId()
        {
            var userIdClaim = User.FindFirst(JwtRegisteredClaimNames.Sub)?.Value
                ?? User.FindFirst(ClaimTypes.NameIdentifier)?.Value
                ?? throw new UnauthorizedAccessException("User ID not found in claims");

            return Guid.Parse(userIdClaim);
        }

        [HttpPost]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateBooking([FromBody] CreateBookingCommand command, CancellationToken ct)
        {
            var userId = GetUserId();
            var commandWithUserId = command with { UserId = userId };

            var result = await _mediator.Send(commandWithUserId, ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetBooking(Guid id, CancellationToken ct)
        {
            var query = new GetBookingQuery(id);
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpGet("my")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        public async Task<IActionResult> GetMyBookings(CancellationToken ct)
        {
            var userId = GetUserId();
            var query = new GetMyBookingsQuery(userId);
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpGet("tour/{tourId}")]
        [Authorize(Roles = "TourOperator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTourBookings(Guid tourId, CancellationToken ct)
        {
            var operatorId = GetUserId();
            var query = new GetTourBookingsQuery(tourId, operatorId);
            var result = await _mediator.Send(query, ct);
            return Ok(result);
        }

        [HttpPost("{id}/confirm")]
        [Authorize(Roles = "TourOperator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> ConfirmBooking(Guid id, CancellationToken ct)
        {
            var operatorId = GetUserId();
            var command = new ConfirmBookingCommand(id, operatorId);
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        [HttpPost("{id}/pay")]
        [Authorize(Roles = "Tourist")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> PayBooking(Guid id, CancellationToken ct)
        {
            var userId = GetUserId();
            var command = new PayBookingCommand(id, userId);
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        [HttpPost("{id}/cancel")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CancelBooking(Guid id, CancellationToken ct)
        {
            var requesterId = GetUserId();
            var command = new CancelBookingCommand(id, requesterId);
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
    }
}
