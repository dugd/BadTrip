using BadTrip.Domain.Common;
using BadTrip.Domain.Exceptions;
using BadTrip.Domain.ValueObjects;

namespace BadTrip.Domain.Entities;

public class Booking : BaseEntity
{
    private readonly List<Passenger> _passengers = new();

    public Guid UserId { get; private set; }
    public Guid TourId { get; private set; }
    public IReadOnlyList<Passenger> Passengers => _passengers.AsReadOnly();
    public Money TotalPrice { get; private set; }
    public BookingStatus Status { get; private set; }

    // Navigation properties
    public User User { get; private set; } = null!;
    public Tour Tour { get; private set; } = null!;

    protected Booking()
    {
        TotalPrice = new Money(0, "USD");
    }

    private Booking(Guid userId, Guid tourId, List<Passenger> passengers, Money totalPrice)
        : this()
    {
        UserId = userId;
        TourId = tourId;
        _passengers = passengers ?? throw new ArgumentNullException(nameof(passengers));
        TotalPrice = totalPrice ?? throw new ArgumentNullException(nameof(totalPrice));
        Status = BookingStatus.Pending;
    }

    public static Booking Create(Guid userId, Guid tourId, List<Passenger> passengers, Money priceSnapshot)
    {
        if (passengers == null || passengers.Count == 0)
            throw new ValidationException("At least one passenger is required");

        if (passengers.Count > 10)
            throw new ValidationException("Maximum 10 passengers allowed per booking");

        if (!passengers.Any(p => p.IsAdult()))
            throw new ValidationException("At least one adult passenger (18+) is required");

        if (priceSnapshot == null || priceSnapshot.Amount <= 0)
            throw new ValidationException("Price must be greater than zero");

        return new Booking(userId, tourId, passengers, priceSnapshot);
    }

    public void Confirm()
    {
        if (Status != BookingStatus.Pending)
            throw new DomainException($"Cannot confirm booking in {Status} status. Only Pending bookings can be confirmed.");

        Status = BookingStatus.Confirmed;
    }

    public void Pay()
    {
        if (Status != BookingStatus.Confirmed)
            throw new DomainException($"Cannot pay for booking in {Status} status. Only Confirmed bookings can be paid.");

        Status = BookingStatus.Paid;
    }

    public void Cancel()
    {
        if (Status == BookingStatus.Paid)
            throw new DomainException("Cannot cancel a paid booking. Please request a refund.");

        if (Status == BookingStatus.Cancelled)
            throw new DomainException("Booking is already cancelled");

        Status = BookingStatus.Cancelled;
    }

    public int GetPassengerCount() => _passengers.Count;
}
