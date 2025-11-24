using FluentValidation;

namespace BadTrip.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandValidator : AbstractValidator<CancelBookingCommand>
{
    public CancelBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required");

        RuleFor(x => x.RequesterId)
            .NotEmpty().WithMessage("Requester ID is required");
    }
}
