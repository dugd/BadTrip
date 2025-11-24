using BadTrip.Application.Features.Tours.DTO;

namespace BadTrip.Application.Features.Bookings.DTO;

public record BookingResponse(
    Guid Id,
    Guid UserId,
    string UserName,
    Guid TourId,
    string TourTitle,
    IReadOnlyList<PassengerDto> Passengers,
    MoneyDto TotalPrice,
    string Status,
    DateTime CreatedAt,
    string? TransactionId = null
);
