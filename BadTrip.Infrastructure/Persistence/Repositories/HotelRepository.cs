using BadTrip.Domain.Entities;
using BadTrip.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BadTrip.Infrastructure.Persistence.Repositories
{
    public class HotelRepository : Repository<Hotel>, IHotelRepository
    {
        public HotelRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null)
        {
            if (excludeId.HasValue)
            {
                return !await _dbSet.AnyAsync(h => h.Name == name && h.Id != excludeId.Value);
            }

            return !await _dbSet.AnyAsync(h => h.Name == name);
        }
    }
}
