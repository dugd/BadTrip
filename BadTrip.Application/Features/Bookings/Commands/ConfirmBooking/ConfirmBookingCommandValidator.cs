using FluentValidation;

namespace BadTrip.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandValidator : AbstractValidator<ConfirmBookingCommand>
{
    public ConfirmBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required");

        RuleFor(x => x.OperatorId)
            .NotEmpty().WithMessage("Operator ID is required");
    }
}
