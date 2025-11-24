using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.Entities;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Queries.GetTourBookings;

public class GetTourBookingsQueryHandler : IRequestHandler<GetTourBookingsQuery, IReadOnlyList<BookingResponse>>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly ITourRepository _tourRepository;

    public GetTourBookingsQueryHandler(
        IBookingRepository bookingRepository,
        ITourRepository tourRepository)
    {
        _bookingRepository = bookingRepository;
        _tourRepository = tourRepository;
    }

    public async Task<IReadOnlyList<BookingResponse>> Handle(GetTourBookingsQuery request, CancellationToken cancellationToken)
    {
        // Verify tour exists and operator owns it
        var tour = await _tourRepository.GetByIdAsync(request.TourId)
            ?? throw new NotFoundException(nameof(Tour), request.TourId);

        if (tour.OperatorId != request.OperatorId)
            throw new ForbiddenException("You can only view bookings for your own tours");

        var bookings = await _bookingRepository.GetTourBookingsAsync(request.TourId);

        return bookings.Select(booking => new BookingResponse(
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
        )).ToList();
    }
}
