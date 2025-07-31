using _03.QuotationService.Entities;
using _03.QuotationService.Data;
using Microsoft.EntityFrameworkCore;


namespace _03.QuotationService.Repositories
{
    public class QuotationRepository : IQuotationRepository
    {
        private readonly ApplicationDbContext _db;
        public QuotationRepository(ApplicationDbContext db) => _db = db;

        public async Task AddAsync(Quotation quote)
        {
            _db.Quotations.Add(quote);
            await _db.SaveChangesAsync();
        }

        public async Task<IEnumerable<Quotation>> GetByOrderAsync(Guid orderId)
        {
            return await _db.Quotations
                .Include(q => q.Items)
                .Where(q => q.OrderId == orderId)
                .ToListAsync();
        }
    }
}
