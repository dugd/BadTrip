using BadTrip.Domain.Entities;
using BadTrip.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BadTrip.Infrastructure.Persistence.Repositories;

public class BookingRepository : Repository<Booking>, IBookingRepository
{
    public BookingRepository(ApplicationDbContext context) : base(context)
    {
    }

    public override async Task<Booking?> GetByIdAsync(Guid id)
    {
        return await _context.Set<Booking>()
            .Include(b => b.User)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Hotel)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Operator)
            .FirstOrDefaultAsync(b => b.Id == id);
    }

    public async Task<Booking?> GetByIdWithDetailsAsync(Guid id)
    {
        return await GetByIdAsync(id);
    }

    public async Task<IReadOnlyList<Booking>> GetUserBookingsAsync(Guid userId)
    {
        return await _context.Set<Booking>()
            .Include(b => b.User)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Hotel)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Operator)
            .Where(b => b.UserId == userId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<IReadOnlyList<Booking>> GetTourBookingsAsync(Guid tourId)
    {
        return await _context.Set<Booking>()
            .Include(b => b.User)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Hotel)
            .Include(b => b.Tour)
                .ThenInclude(t => t.Operator)
            .Where(b => b.TourId == tourId)
            .OrderByDescending(b => b.CreatedAt)
            .ToListAsync();
    }

    public async Task<bool> HasActiveBookingForTourAsync(Guid userId, Guid tourId)
    {
        return await _context.Set<Booking>()
            .AnyAsync(b => b.UserId == userId
                        && b.TourId == tourId
                        && b.Status != BookingStatus.Cancelled);
    }
}
