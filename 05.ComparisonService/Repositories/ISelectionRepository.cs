using _05.ComparisonService.Entities;

namespace _05.ComparisonService.Repositories
{
    public interface ISelectionRepository
    {
        Task AddRangeAsync(IEnumerable<Selection> selections);
        Task<IEnumerable<Selection>> GetByOrderAsync(Guid orderId);
    }
}
