using BadTrip.Application.Features.Users.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Users.Queries.GetPublicUser;

public class GetPublicUserQueryHandler : IRequestHandler<GetPublicUserQuery, PublicUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetPublicUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<PublicUserResponse> Handle(
        GetPublicUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        return new PublicUserResponse(
            user.Id,
            user.FirstName,
            user.LastName,
            user.Role.ToString(),
            user.CompanyName
        );
    }
}
