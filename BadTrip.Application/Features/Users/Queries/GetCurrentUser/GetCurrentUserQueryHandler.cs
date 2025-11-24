using BadTrip.Application.Features.Users.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Users.Queries.GetCurrentUser;

public class GetCurrentUserQueryHandler : IRequestHandler<GetCurrentUserQuery, CurrentUserResponse>
{
    private readonly IUserRepository _userRepository;

    public GetCurrentUserQueryHandler(IUserRepository userRepository)
    {
        _userRepository = userRepository;
    }

    public async Task<CurrentUserResponse> Handle(
        GetCurrentUserQuery request,
        CancellationToken cancellationToken
    )
    {
        var user = await _userRepository.GetByIdAsync(request.UserId);

        if (user == null)
        {
            throw new NotFoundException(nameof(User), request.UserId);
        }

        return new CurrentUserResponse(
            user.Id,
            user.Email,
            user.Role.ToString(),
            user.FirstName,
            user.LastName,
            user.PhoneNumber,
            user.DateOfBirth,
            user.CompanyName,
            user.CreatedAt,
            user.UpdatedAt
        );
    }
}
