using FluentValidation;

namespace BadTrip.Application.Features.Hotels.Queries.GetHotel
{
    public class GetHotelQueryValidator : AbstractValidator<GetHotelQuery>
    {
        public GetHotelQueryValidator()
        {
            RuleFor(x => x.Id)
                .NotEmpty().WithMessage("Hotel ID is required.");
        }
    }
}
