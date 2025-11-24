using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.Entities;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Queries.GetBooking;

public class GetBookingQueryHandler : IRequestHandler<GetBookingQuery, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;

    public GetBookingQueryHandler(IBookingRepository bookingRepository)
    {
        _bookingRepository = bookingRepository;
    }

    public async Task<BookingResponse> Handle(GetBookingQuery request, CancellationToken cancellationToken)
    {
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

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
