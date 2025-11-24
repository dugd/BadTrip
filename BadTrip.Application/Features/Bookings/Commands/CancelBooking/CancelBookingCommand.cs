using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.CancelBooking;

public record CancelBookingCommand(
    Guid BookingId,
    Guid RequesterId
) : IRequest<BookingResponse>;
