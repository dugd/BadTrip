using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.ValueObjects;
using MediatR;

namespace BadTrip.Application.Features.Tours.Commands.UpdateTour
{
    public record UpdateTourCommand(
        Guid Id,
        string Title,
        string Description,
        decimal PriceAmount,
        string PriceCurrency,
        string ImageUrl,
        Guid? HotelId,
        int MaxParticipants,
        DateTime StartDate,
        DateTime EndDate
    ) : IRequest<TourResponse>;

    public class UpdateTourCommandHandler : IRequestHandler<UpdateTourCommand, TourResponse>
    {
        private readonly ITourRepository _tourRepo;
        private readonly IHotelRepository _hotelRepo;
        private readonly IUnitOfWork _unitOfWork;

        public UpdateTourCommandHandler(
            ITourRepository tourRepo,
            IHotelRepository hotelRepo,
            IUnitOfWork unitOfWork)
        {
            _tourRepo = tourRepo;
            _hotelRepo = hotelRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<TourResponse> Handle(UpdateTourCommand request, CancellationToken cancellationToken)
        {
            // Get existing tour
            var tour = await _tourRepo.GetByIdAsync(request.Id);
            if (tour == null)
            {
                throw new NotFoundException(nameof(Tour), request.Id);
            }

            // Check title uniqueness (excluding current tour)
            if (!await _tourRepo.IsTitleUniqueAsync(request.Title, request.Id))
            {
                throw new DomainException($"Tour with title '{request.Title}' already exists.");
            }

            // If hotel is specified, verify it exists
            if (request.HotelId.HasValue)
            {
                var hotel = await _hotelRepo.GetByIdAsync(request.HotelId.Value);
                if (hotel == null)
                {
                    throw new NotFoundException(nameof(Hotel), request.HotelId.Value);
                }
            }

            // Create Money Value Object
            var price = new Money(request.PriceAmount, request.PriceCurrency);

            // Update tour via domain method
            tour.Update(
                request.Title,
                request.Description,
                price,
                request.ImageUrl,
                request.HotelId,
                request.MaxParticipants,
                request.StartDate,
                request.EndDate
            );

            // Save
            _tourRepo.Update(tour);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload to get navigation properties
            var updatedTour = await _tourRepo.GetByIdAsync(tour.Id);

            // Return response
            return new TourResponse(
                updatedTour!.Id,
                updatedTour.Title,
                updatedTour.Description,
                new MoneyDto(updatedTour.Price.Amount, updatedTour.Price.Currency),
                updatedTour.ImageUrl,
                updatedTour.HotelId,
                updatedTour.Hotel?.Name,
                updatedTour.MaxParticipants,
                updatedTour.SoldSpots,
                updatedTour.AvailableSpots,
                updatedTour.StartDate,
                updatedTour.EndDate,
                updatedTour.OperatorId,
                $"{updatedTour.Operator.FirstName} {updatedTour.Operator.LastName}",
                updatedTour.CreatedAt
            );
        }
    }
}
