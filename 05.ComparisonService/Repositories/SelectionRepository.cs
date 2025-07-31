using _05.ComparisonService.Entities;
using Microsoft.EntityFrameworkCore; // Add this for ToListAsync()
using _05.ComparisonService.Data;    // Add this for ApplicationDbContext

namespace _05.ComparisonService.Repositories
{
    public class SelectionRepository : ISelectionRepository
    {
        private readonly ApplicationDbContext _db;
        public SelectionRepository(ApplicationDbContext db) => _db = db;

        public async Task AddRangeAsync(IEnumerable<Selection> selections)
        {
            _db.Selections.AddRange(selections);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Selection>> GetByOrderAsync(Guid orderId)
        {
            return await _db.Selections
                .Where(s => s.OrderId == orderId)
                .ToListAsync();
        }
    }
}
