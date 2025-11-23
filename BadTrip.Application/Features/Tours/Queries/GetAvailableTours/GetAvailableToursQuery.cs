using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Tours.Queries.GetAvailableTours
{
    public record GetAvailableToursQuery : IRequest<IReadOnlyList<TourResponse>>;

    public class GetAvailableToursQueryHandler : IRequestHandler<GetAvailableToursQuery, IReadOnlyList<TourResponse>>
    {
        private readonly ITourRepository _tourRepo;

        public GetAvailableToursQueryHandler(ITourRepository tourRepo)
        {
            _tourRepo = tourRepo;
        }

        public async Task<IReadOnlyList<TourResponse>> Handle(GetAvailableToursQuery request, CancellationToken cancellationToken)
        {
            var tours = await _tourRepo.GetAvailableToursAsync();

            return tours.Select(tour => new TourResponse(
                tour.Id,
                tour.Title,
                tour.Description,
                new MoneyDto(tour.Price.Amount, tour.Price.Currency),
                tour.ImageUrl,
                tour.HotelId,
                tour.Hotel?.Name,
                tour.MaxParticipants,
                tour.SoldSpots,
                tour.AvailableSpots,
                tour.StartDate,
                tour.EndDate,
                tour.OperatorId,
                $"{tour.Operator.FirstName} {tour.Operator.LastName}",
                tour.CreatedAt
            )).ToList();
        }
    }
}
