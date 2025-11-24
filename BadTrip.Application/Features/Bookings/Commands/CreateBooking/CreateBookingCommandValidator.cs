using FluentValidation;

namespace BadTrip.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandValidator : AbstractValidator<CreateBookingCommand>
{
    public CreateBookingCommandValidator()
    {
        RuleFor(x => x.UserId)
            .NotEmpty().WithMessage("User ID is required");

        RuleFor(x => x.TourId)
            .NotEmpty().WithMessage("Tour ID is required");

        RuleFor(x => x.Passengers)
            .NotEmpty().WithMessage("At least one passenger is required")
            .Must(p => p != null && p.Count > 0).WithMessage("At least one passenger is required")
            .Must(p => p != null && p.Count <= 10).WithMessage("Maximum 10 passengers allowed per booking");

        RuleForEach(x => x.Passengers).ChildRules(passenger =>
        {
            passenger.RuleFor(p => p.FirstName)
                .NotEmpty().WithMessage("Passenger first name is required")
                .MaximumLength(100).WithMessage("First name must not exceed 100 characters");

            passenger.RuleFor(p => p.LastName)
                .NotEmpty().WithMessage("Passenger last name is required")
                .MaximumLength(100).WithMessage("Last name must not exceed 100 characters");

            passenger.RuleFor(p => p.PassportNumber)
                .NotEmpty().WithMessage("Passenger passport number is required")
                .MaximumLength(50).WithMessage("Passport number must not exceed 50 characters");

            passenger.RuleFor(p => p.DateOfBirth)
                .NotEmpty().WithMessage("Passenger date of birth is required")
                .LessThan(DateTime.UtcNow).WithMessage("Date of birth must be in the past");
        });
    }
}
