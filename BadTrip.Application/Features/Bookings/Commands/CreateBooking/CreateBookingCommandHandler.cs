using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Enums;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.ValueObjects;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.CreateBooking;

public class CreateBookingCommandHandler : IRequestHandler<CreateBookingCommand, BookingResponse>
{
    private readonly IUserRepository _userRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IBookingRepository _bookingRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CreateBookingCommandHandler(
        IUserRepository userRepository,
        ITourRepository tourRepository,
        IBookingRepository bookingRepository,
        IUnitOfWork unitOfWork)
    {
        _userRepository = userRepository;
        _tourRepository = tourRepository;
        _bookingRepository = bookingRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponse> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Verify user exists and is a Tourist
        var user = await _userRepository.GetByIdAsync(request.UserId)
            ?? throw new NotFoundException(nameof(User), request.UserId);

        if (user.Role != UserRole.Tourist)
            throw new ForbiddenException("Only tourists can create bookings");

        // 2. Load tour with details
        var tour = await _tourRepository.GetByIdAsync(request.TourId)
            ?? throw new NotFoundException(nameof(Tour), request.TourId);

        // 3. Check if user already has an active booking for this tour
        var hasActiveBooking = await _bookingRepository.HasActiveBookingForTourAsync(request.UserId, request.TourId);
        if (hasActiveBooking)
            throw new DomainException("You already have an active booking for this tour");

        // 4. Check available spots
        var passengerCount = request.Passengers.Count;
        if (tour.AvailableSpots < passengerCount)
            throw new DomainException($"Not enough available spots. Tour has {tour.AvailableSpots} spots available");

        // 5. Reserve spots on tour (handles optimistic concurrency via RowVersion)
        tour.ReserveSpots(passengerCount);

        // 6. Convert PassengerDto to Passenger value objects
        var passengers = request.Passengers.Select(p =>
            new Passenger(p.FirstName, p.LastName, p.PassportNumber, p.DateOfBirth)
        ).ToList();

        // 7. Calculate total price (price per person * passenger count)
        var totalPrice = tour.Price * passengerCount;

        // 8. Create booking entity
        var booking = Booking.Create(
            request.UserId,
            request.TourId,
            passengers,
            totalPrice
        );

        // 9. Save booking and tour in same transaction
        await _bookingRepository.AddAsync(booking);
        _tourRepository.Update(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 10. Map to response
        return MapToResponse(booking, user, tour);
    }

    private static BookingResponse MapToResponse(Booking booking, User user, Tour tour)
    {
        return new BookingResponse(
            Id: booking.Id,
            UserId: booking.UserId,
            UserName: $"{user.FirstName} {user.LastName}",
            TourId: booking.TourId,
            TourTitle: tour.Title,
            Passengers: booking.Passengers.Select(p => new PassengerDto(
                p.FirstName,
                p.LastName,
                p.PassportNumber,
                p.DateOfBirth
            )).ToList(),
            TotalPrice: new MoneyDto(booking.TotalPrice.Amount, booking.TotalPrice.Currency),
            Status: booking.Status.ToString(),
            CreatedAt: booking.CreatedAt
        );
    }
}
