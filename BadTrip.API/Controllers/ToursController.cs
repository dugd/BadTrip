using Azure.Core;
using BadTrip.API.Contracts.Tours;
using BadTrip.API.Extensions;
using BadTrip.Application.Features.Tours.Commands.CreateTour;
using BadTrip.Application.Features.Tours.Commands.UpdateTour;
using BadTrip.Application.Features.Tours.Queries.GetAllTours;
using BadTrip.Application.Features.Tours.Queries.GetAvailableTours;
using BadTrip.Application.Features.Tours.Queries.GetTour;
using BadTrip.Application.Features.Tours.Queries.GetToursByOperator;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadTrip.API.Controllers
{
    [ApiController]
    [Route("api/tours")]
    public class ToursController : ControllerBase
    {
        private readonly IMediator _mediator;

        public ToursController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost]
        [Authorize(Roles = "TourOperator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> CreateTour([FromBody] CreateTourRequest request, CancellationToken ct)
        {
            var command = new CreateTourCommand(
                Title: request.Title,
                Description: request.Description,
                PriceAmount: request.PriceAmount,
                PriceCurrency: request.PriceCurrency,
                ImageUrl: request.ImageUrl,
                HotelId: request.HotelId,
                MaxParticipants: request.MaxParticipants,
                StartDate: request.StartDate,
                EndDate: request.EndDate,
                OperatorId: User.GetUserId()
            );
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }

        [HttpGet]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAllTours(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAllToursQuery(), ct);
            return Ok(result);
        }

        [HttpGet("{id}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public async Task<IActionResult> GetTour(Guid id, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetTourQuery(id), ct);
            return Ok(result);
        }

        [HttpGet("available")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetAvailableTours(CancellationToken ct)
        {
            var result = await _mediator.Send(new GetAvailableToursQuery(), ct);
            return Ok(result);
        }

        [HttpGet("operator/{operatorId}")]
        [AllowAnonymous]
        [ProducesResponseType(StatusCodes.Status200OK)]
        public async Task<IActionResult> GetToursByOperator(Guid operatorId, CancellationToken ct)
        {
            var result = await _mediator.Send(new GetToursByOperatorQuery(operatorId), ct);
            return Ok(result);
        }

        [HttpPut("{id}")]
        [Authorize(Roles = "TourOperator")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status401Unauthorized)]
        [ProducesResponseType(StatusCodes.Status403Forbidden)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> UpdateTour(Guid id, [FromBody] UpdateTourRequest request, CancellationToken ct)
        {
            var command = new UpdateTourCommand(
                Id: id,
                Title: request.Title,
                Description: request.Description,
                PriceAmount: request.PriceAmount,
                PriceCurrency: request.PriceCurrency,
                ImageUrl: request.ImageUrl,
                HotelId: request.HotelId,
                MaxParticipants: request.MaxParticipants,
                StartDate: request.StartDate,
                EndDate: request.EndDate
                // OperatorId: operatorId
            );
            var result = await _mediator.Send(command, ct);
            return Ok(result);
        }
    }
}
