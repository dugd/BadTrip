using BadTrip.Application.Features.Users.DTO;
using MediatR;

namespace BadTrip.Application.Features.Users.Queries.GetCurrentUser;

public record GetCurrentUserQuery(Guid UserId) : IRequest<CurrentUserResponse>;
