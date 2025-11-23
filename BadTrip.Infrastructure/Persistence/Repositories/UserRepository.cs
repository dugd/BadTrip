using BadTrip.Domain.Entities;
using BadTrip.Domain.Interfaces.Repositories;
using Microsoft.EntityFrameworkCore;

namespace BadTrip.Infrastructure.Persistence.Repositories
{
    public class UserRepository : Repository<User>, IUserRepository
    {
        public UserRepository(ApplicationDbContext context) : base(context)
        {
        }

        public async Task<User?> GetByEmailAsync(string email)
        {
            return await _dbSet.Where(u => u.Email == email).FirstOrDefaultAsync();
        }

        public async Task<bool> IsEmailUniqueAsync(string email)
        {
            return !await _dbSet.AnyAsync(u => u.Email == email);
        }

        public async Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber)
        {
            return !await _dbSet.AnyAsync(u => u.PhoneNumber == phoneNumber);
        }
    }
}
