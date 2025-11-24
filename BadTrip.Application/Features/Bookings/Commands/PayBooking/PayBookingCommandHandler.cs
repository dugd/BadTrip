using BadTrip.Application.Features.Bookings.DTO;
using BadTrip.Application.Features.Tours.DTO;
using BadTrip.Domain.Entities;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.Interfaces;
using BadTrip.Domain.Interfaces.Repositories;
using BadTrip.Domain.Services;
using MediatR;

namespace BadTrip.Application.Features.Bookings.Commands.PayBooking;

public class PayBookingCommandHandler : IRequestHandler<PayBookingCommand, BookingResponse>
{
    private readonly IBookingRepository _bookingRepository;
    private readonly IPaymentService _paymentService;
    private readonly IUnitOfWork _unitOfWork;

    public PayBookingCommandHandler(
        IBookingRepository bookingRepository,
        IPaymentService paymentService,
        IUnitOfWork unitOfWork)
    {
        _bookingRepository = bookingRepository;
        _paymentService = paymentService;
        _unitOfWork = unitOfWork;
    }

    public async Task<BookingResponse> Handle(PayBookingCommand request, CancellationToken cancellationToken)
    {
        // 1. Load booking with all details
        var booking = await _bookingRepository.GetByIdWithDetailsAsync(request.BookingId)
            ?? throw new NotFoundException(nameof(Booking), request.BookingId);

        // 2. Verify user owns the booking
        if (booking.UserId != request.UserId)
            throw new ForbiddenException("You can only pay for your own bookings");

        // 3. Process payment via payment service
        var paymentResult = await _paymentService.ProcessPaymentAsync(booking.TotalPrice);

        if (!paymentResult.Success)
            throw new DomainException($"Payment failed: {paymentResult.FailureReason}");

        // 4. Mark booking as paid (throws if not in Confirmed status)
        booking.Pay();

        // 5. Save changes
        _bookingRepository.Update(booking);
        await _unitOfWork.SaveChangesAsync(cancellationToken);

        // 6. Map to response with transaction ID
        return MapToResponse(booking, paymentResult.TransactionId);
    }

    private static BookingResponse MapToResponse(Booking booking, string? transactionId)
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
            CreatedAt: booking.CreatedAt,
            TransactionId: transactionId
        );
    }
}
