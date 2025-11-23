using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.ValueObjects;
using MediatR;

namespace BadTrip.Application.Features.Tours.Commands.CreateTour
{
    public record CreateTourCommand(
        string Title,
        string Description,
        decimal PriceAmount,
        string PriceCurrency,
        string ImageUrl,
        Guid? HotelId,
        int MaxParticipants,
        DateTime StartDate,
        DateTime EndDate,
        Guid OperatorId
    ) : IRequest<TourResponse>;

    public class CreateTourCommandHandler : IRequestHandler<CreateTourCommand, TourResponse>
    {
        private readonly ITourRepository _tourRepo;
        private readonly IHotelRepository _hotelRepo;
        private readonly IUserRepository _userRepo;
        private readonly IUnitOfWork _unitOfWork;

        public CreateTourCommandHandler(
            ITourRepository tourRepo,
            IHotelRepository hotelRepo,
            IUserRepository userRepo,
            IUnitOfWork unitOfWork)
        {
            _tourRepo = tourRepo;
            _hotelRepo = hotelRepo;
            _userRepo = userRepo;
            _unitOfWork = unitOfWork;
        }

        public async Task<TourResponse> Handle(CreateTourCommand request, CancellationToken cancellationToken)
        {
            // Check title uniqueness
            if (!await _tourRepo.IsTitleUniqueAsync(request.Title))
            {
                throw new DomainException($"Tour with title '{request.Title}' already exists.");
            }

            // Verify operator exists and is TourOperator
            var operatorUser = await _userRepo.GetByIdAsync(request.OperatorId);
            if (operatorUser == null)
            {
                throw new NotFoundException(nameof(User), request.OperatorId);
            }

            if (operatorUser.Role != Domain.Enums.UserRole.TourOperator)
            {
                throw new ForbiddenException("Only TourOperators can create tours.");
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

            // Create Tour entity via factory method
            var tour = Tour.Create(
                request.Title,
                request.Description,
                price,
                request.ImageUrl,
                request.HotelId,
                request.MaxParticipants,
                request.StartDate,
                request.EndDate,
                request.OperatorId
            );

            // Save
            await _tourRepo.AddAsync(tour);
            await _unitOfWork.SaveChangesAsync(cancellationToken);

            // Reload to get navigation properties
            var savedTour = await _tourRepo.GetByIdAsync(tour.Id);

            // Return response
            return new TourResponse(
                savedTour!.Id,
                savedTour.Title,
                savedTour.Description,
                new MoneyDto(savedTour.Price.Amount, savedTour.Price.Currency),
                savedTour.ImageUrl,
                savedTour.HotelId,
                savedTour.Hotel?.Name,
                savedTour.MaxParticipants,
                savedTour.SoldSpots,
                savedTour.AvailableSpots,
                savedTour.StartDate,
                savedTour.EndDate,
                savedTour.OperatorId,
                $"{savedTour.Operator.FirstName} {savedTour.Operator.LastName}",
                savedTour.CreatedAt
            );
        }
    }
}
