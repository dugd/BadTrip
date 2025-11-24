using BadTrip.Application.Features.Bookings.DTO;

namespace BadTrip.API.Contracts.Booking
{
    public record CreateBookingRequest(
        Guid TourId,
        List<PassengerDto> Passengers
        );
}
