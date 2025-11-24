using FluentValidation;

namespace BadTrip.Application.Features.Bookings.Commands.PayBooking;

public class PayBookingCommandValidator : AbstractValidator<PayBookingCommand>
{
    public PayBookingCommandValidator()
    {
        RuleFor(x => x.BookingId)
            .NotEmpty().WithMessage("Booking ID is required");

        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");
    }
}
