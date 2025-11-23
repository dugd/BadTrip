using BadTrip.Application.Features.Auth.Commands;
using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace BadTrip.API.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IMediator _mediator;

        public AuthController(IMediator mediator)
        {
            _mediator = mediator;
        }

        [HttpPost("register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status409Conflict)]
        public async Task<IActionResult> Register([FromBody] RegisterUserCommand command, CancellationToken ct)
        {
            await _mediator.Send(command, ct);
            return Ok(new { Message = "Registration successful" });
        }

        [HttpPost("login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<IActionResult> Login([FromBody] LoginUserCommand command, CancellationToken ct)
        {
            var res = await _mediator.Send(command, ct);
            return Ok(res);
        }
    }
}
