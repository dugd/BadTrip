using FluentValidation;

namespace BadTrip.Application.Features.Tours.Commands.UpdateTour
{
    public class UpdateTourCommandValidator : AbstractValidator<UpdateTourCommand>
    {
        public UpdateTourCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Tour ID is required.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(200).WithMessage("Title cannot exceed 200 characters.");

            RuleFor(x => x.Description)
                .NotEmpty().WithMessage("Description is required.")
                .MaximumLength(2000).WithMessage("Description cannot exceed 2000 characters.");

            RuleFor(x => x.PriceAmount)
                .GreaterThan(0).WithMessage("Price amount must be greater than zero.");

            RuleFor(x => x.PriceCurrency)
                .NotEmpty().WithMessage("Price currency is required.")
                .Length(3).WithMessage("Currency must be a 3-letter ISO code (e.g., USD, EUR).")
                .Matches("^[A-Z]{3}$").WithMessage("Currency must be uppercase letters only.");

            RuleFor(x => x.ImageUrl)
                .NotEmpty().WithMessage("Image URL is required.")
                .Must(BeAValidUrl).WithMessage("Image URL must be a valid absolute URL.");

            RuleFor(x => x.MaxParticipants)
                .GreaterThan(0).WithMessage("Max participants must be greater than zero.")
                .LessThanOrEqualTo(1000).WithMessage("Max participants cannot exceed 1000.");

            RuleFor(x => x.StartDate)
                .GreaterThan(DateTime.UtcNow).WithMessage("Start date must be in the future.");

            RuleFor(x => x.EndDate)
                .GreaterThan(x => x.StartDate).WithMessage("End date must be after start date.");
        }

        private bool BeAValidUrl(string url)
        {
            return Uri.TryCreate(url, UriKind.Absolute, out _);
        }
    }
}
