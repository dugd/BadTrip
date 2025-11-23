using BadTrip.Domain.Enums;

namespace BadTrip.Application.Features.Auth.DTO
{
    public record AuthResponse(
        Guid Id,
        string Email,
        string Token,
        UserRole Role
        );
}
