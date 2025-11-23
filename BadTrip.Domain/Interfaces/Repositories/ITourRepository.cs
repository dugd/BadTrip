using BadTrip.Domain.Entities;

namespace BadTrip.Domain.Interfaces.Repositories
{
    public interface ITourRepository : IRepository<Tour>
    {
        Task<IReadOnlyList<Tour>> GetByOperatorIdAsync(Guid operatorId);
        Task<IReadOnlyList<Tour>> GetAvailableToursAsync();
        Task<bool> IsTitleUniqueAsync(string title, Guid? excludeId = null);
    }
}
