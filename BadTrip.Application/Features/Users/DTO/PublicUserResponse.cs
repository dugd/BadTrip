namespace BadTrip.Application.Features.Users.DTO;

public record PublicUserResponse(
    Guid Id,
    string FirstName,
    string LastName,
    string Role,
    string? CompanyName
);
