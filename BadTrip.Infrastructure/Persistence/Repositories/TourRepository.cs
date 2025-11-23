using BadTrip.Domain.Entities;
using BadTrip.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BadTrip.Infrastructure.Persistence.Repositories
{
    public class TourRepository : Repository<Tour>, ITourRepository
    {
        public TourRepository(ApplicationDbContext context) : base(context)
        {
        }

        public override async Task<Tour?> GetByIdAsync(Guid id)
        {
            return await _context.Tours
                .Include(t => t.Hotel)
                .Include(t => t.Operator)
                .FirstOrDefaultAsync(t => t.Id == id);
        }

        public override async Task<IReadOnlyList<Tour>> GetAllAsync()
        {
            return await _context.Tours
                .Include(t => t.Hotel)
                .Include(t => t.Operator)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Tour>> GetByOperatorIdAsync(Guid operatorId)
        {
            return await _context.Tours
                .Include(t => t.Hotel)
                .Include(t => t.Operator)
                .Where(t => t.OperatorId == operatorId)
                .OrderByDescending(t => t.CreatedAt)
                .ToListAsync();
        }

        public async Task<IReadOnlyList<Tour>> GetAvailableToursAsync()
        {
            var now = DateTime.UtcNow;

            return await _context.Tours
                .Include(t => t.Hotel)
                .Include(t => t.Operator)
                .Where(t => t.StartDate > now && t.MaxParticipants > t.SoldSpots)
                .OrderBy(t => t.StartDate)
                .ToListAsync();
        }

        public override async Task AddAsync(Tour tour)
        {
            await _context.Tours.AddAsync(tour);
        }

        public override void Update(Tour tour)
        {
            _context.Tours.Update(tour);
        }

        public async Task<bool> IsTitleUniqueAsync(string title, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _context.Tours.AnyAsync(t => t.Title == title && t.Id != excludeId.Value);
            }

            return !await _context.Tours.AnyAsync(t => t.Title == title);
        }
    }
}
