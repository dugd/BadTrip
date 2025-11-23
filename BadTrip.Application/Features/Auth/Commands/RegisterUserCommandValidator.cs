using BadTrip.Application.Common.Validation;
using BadTrip.Domain.Enums;
using FluentValidation;

namespace BadTrip.Application.Features.Auth.Commands
{
    public class RegisterUserCommandValidator : AbstractValidator<RegisterUserCommand>
    {
        public RegisterUserCommandValidator()
        {
            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.");

            RuleFor(x => x.Password)
                .NotEmpty()
                .MinimumLength(8).WithMessage("Password must be at least 8 characters long.")
                .Matches("[A-Z]").WithMessage("Password must contain at least one uppercase letter.")
                .Matches("[0-9]").WithMessage("Password must contain at least one number.");

            RuleFor(x => x.FirstName).NotEmpty().MaximumLength(50);
            RuleFor(x => x.LastName).NotEmpty().MaximumLength(50);

            RuleFor(x => x.PhoneNumber).NotEmpty()
                .Phone().WithMessage("Invalid phone number");

            RuleFor(x => x.Role)
                .Must(r => r == UserRole.TourOperator || r == UserRole.Tourist)
                .WithMessage("Invalid role for public registration.");

            When(x => x.Role == UserRole.TourOperator, () =>
            {
                RuleFor(x => x.CompanyName)
                    .NotEmpty().WithMessage("Company Name is required for Tour Operators.")
                    .MaximumLength(100);
            });

            When(x => x.Role == UserRole.Tourist, () =>
            {
                RuleFor(x => x.DateOfBirth)
                    .NotNull().WithMessage("Date of Birth is required for Tourists.")
                    .Must(BeAtLeast18YearsOld).WithMessage("You must be at least 18 years old to suffer with us.");
            });
        }

        private bool BeAtLeast18YearsOld(DateTime? dateOfBirth)
        {
            if (!dateOfBirth.HasValue) return false;

            var today = DateTime.Today;
            var age = today.Year - dateOfBirth.Value.Year;

            // Correction
            if (dateOfBirth.Value.Date > today.AddYears(-age)) age--;

            return age >= 18;
        }
    }
}
