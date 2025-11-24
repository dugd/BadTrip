using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Enums;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.ConfirmBooking;

public class ConfirmBookingCommandHandler : IRequestHandler<ConfirmBookingCommand, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IUserRepository _userRepository;
    private readonly IUnitOfWork _unitOfWork;

    public ConfirmBookingCommandHandler(
        IBookingRepository bookingRepository,
        IUserRepository userRepository,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _userRepository = userRepository;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponse> Handle(ConfirmBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Load booking with all details
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // 2. Verify operator exists and has TourOperator role
        var oper = await _userRepository.GetByIdAsync(request.OperatorId)
            ?? throw new NotFoundException(nameof(User), request.OperatorId);

        if (@oper.Role != UserRole.TourOperator)
            throw new ForbiddenException("Only tour operators can confirm bookings");

        // 3. Verify operator owns the tour
        if (booking.Tour.OperatorId != request.OperatorId)
            throw new ForbiddenException("You can only confirm bookings for your own tours");

        // 4. Confirm the booking (throws if not in Pending status)
        booking.Confirm();

        // 5. Save changes
        _bookingRepository.Update(booking);
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
