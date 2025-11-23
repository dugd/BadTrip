using BadTrip.Domain.Entities;

namespace BadTrip.Domain.Interfaces.Repositories
{
    public interface IHotelRepository : IRepository<Hotel>
    {
        Task<bool> IsNameUniqueAsync(string name, Guid? excludeId = null);
    }
}
