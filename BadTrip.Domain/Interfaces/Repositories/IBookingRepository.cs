using BadTrip.Domain.Entities;

namespace BadTrip.Domain.Interfaces.Repositories;

public interface IBookingRepository : IRepository<Booking>
{
    Task<Booking?> GetByIdWithDetailsAsync(Guid id);
    Task<IReadOnlyList<Booking>> GetUserBookingsAsync(Guid userId);
    Task<IReadOnlyList<Booking>> GetTourBookingsAsync(Guid tourId);
    Task<bool> HasActiveBookingForTourAsync(Guid userId, Guid tourId);
}
