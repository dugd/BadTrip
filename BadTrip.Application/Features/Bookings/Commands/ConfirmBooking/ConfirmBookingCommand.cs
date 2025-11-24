using BadTrip.Application.Features.Bookings.DTO;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.ConfirmBooking;

public record ConfirmBookingCommand(
    Guid BookingId,
    Guid OperatorId
) : IRequest<BookingResponse>;
