using BadTrip.Application.Features.Users.DTO;
using MediatR;

namespace BadTrip.Application.Features.Users.Queries.GetPublicUser;

public record GetPublicUserQuery(Guid UserId) : IRequest<PublicUserResponse>;
