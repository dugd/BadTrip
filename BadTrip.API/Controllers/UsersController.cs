using BadTrip.API.Extensions;
using BadTrip.Application.Features.Users.Queries.GetCurrentUser;
using BadTrip.Application.Features.Users.Queries.GetPublicUser;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BadTrip.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class UsersController : ControllerBase
{
    private readonly IMediator _mediator;

    public UsersController(IMediator mediator)
    {
        _mediator = mediator;
    }

    [HttpGet("me")]
    [Authorize]
    public async Task<IActionResult> GetCurrentUser(CancellationToken ct)
    {
        var userId = User.GetUserId();
        var query = new GetCurrentUserQuery(userId);
        var response = await _mediator.Send(query, ct);
        return Ok(response);
    }

    [HttpGet("{id:guid}")]
    public async Task<IActionResult> GetPublicUser(Guid id, CancellationToken ct)
    {
        var query = new GetPublicUserQuery(id);
        var response = await _mediator.Send(query, ct);
        return Ok(response);
    }
}
