using BadTrip.Domain.Entities;

namespace BadTrip.Domain.Interfaces.Repositories
{
    public interface IUserRepository : IRepository<User>
    {
        Task<User?> GetByEmailAsync(string email);
        Task<bool> IsEmailUniqueAsync(string email);
        Task<bool> IsPhoneNumberUniqueAsync(string phoneNumber);
    }
}
