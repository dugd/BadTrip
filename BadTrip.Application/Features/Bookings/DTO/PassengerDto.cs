namespace BadTrip.Application.Features.Bookings.DTO;

public record PassengerDto(
    string FirstName,
    string LastName,
    string PassportNumber,
    DateTime DateOfBirth
);
