namespace BadTrip.Application.Features.Users.DTO;

public record CurrentUserResponse(
    Guid Id,
    string Email,
    string Role,
    string FirstName,
    string LastName,
    string? PhoneNumber,
    DateTime? DateOfBirth,
    string? CompanyName,
    DateTime CreatedAt,
    DateTime? UpdatedAt
);
