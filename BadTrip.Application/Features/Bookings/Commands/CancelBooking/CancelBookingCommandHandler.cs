using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.CancelBooking;

public class CancelBookingCommandHandler : IRequestHandler<CancelBookingCommand, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITourRepository _tourRepository;
    private readonly IUnitOfWork _unitOfWork;

    public CancelBookingCommandHandler(
        IBookingRepository bookingRepository,
        ITourRepository tourRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _tourRepository = tourRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponse> Handle(CancelBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Load booking with all details
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // 2. Verify requester is authorized (owns booking OR owns tour)
        var isOwner = booking.UserId == request.RequesterId;
        var isOperator = booking.Tour.OperatorId == request.RequesterId;

        if (!isOwner && !isOperator)
            throw new ForbiddenException("You can only cancel your own bookings or bookings for your tours");

        // 3. Cancel the booking (throws if already paid or cancelled)
        booking.Cancel();

        // 4. Return spots to tour
        var tour = booking.Tour;
        tour.ReturnSpots(booking.GetPassengerCount());

        // 5. Save changes
        _bookingRepository.Update(booking);
        _tourRepository.Update(tour);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Map to response
        return MapToResponse(booking);
    }

    private static BookingResponse MapToResponse(Booking booking)
    {
        return new BookingResponse(
            Id: booking.Id,
            UserId: booking.UserId,
            UserName: $"{booking.User.FirstName} {booking.User.LastName}",
            TourId: booking.TourId,
            TourTitle: booking.Tour.Title,
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
