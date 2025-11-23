using FluentValidation;

namespace BadTrip.Application.Features.Hotels.Commands.DeleteHotel
{
    public class DeleteHotelCommandValidator : AbstractValidator<DeleteHotelCommand>
    {
        public DeleteHotelCommandValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hotel ID is required.");
        }
    }
}
