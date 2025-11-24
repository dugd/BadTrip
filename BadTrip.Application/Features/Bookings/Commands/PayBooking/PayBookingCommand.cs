using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.PayBooking;

public record PayBookingCommand(
    Guid BookingId,
    Guid UserId
) : IRequest<BookingResponse>;
