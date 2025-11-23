using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Tours.Queries.GetTour
{
    public record GetTourQuery(Guid Id) : IRequest<TourResponse>;

    public class GetTourQueryHandler : IRequestHandler<GetTourQuery, TourResponse>
    {
        private readonly ITourRepository _tourRepo;

        public GetTourQueryHandler(ITourRepository tourRepo)
        {
            _tourRepo = tourRepo;
        }

        public async Task<TourResponse> Handle(GetTourQuery request, CancellationToken cancellationToken)
        {
            var tour = await _tourRepo.GetByIdAsync(request.Id);
            if (tour == null)
            {
                throw new NotFoundException(nameof(Tour), request.Id);
            }

            return new TourResponse(
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
            );
        }
    }
}
