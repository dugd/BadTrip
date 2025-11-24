using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.CreateBooking;

public record CreateBookingCommand(
    Guid UserId,
    Guid TourId,
    List<PassengerDto> Passengers
) : IRequest<BookingResponse>;
