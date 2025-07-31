using _03.QuotationService.Entities;

namespace _03.QuotationService.Repositories
{
    public interface IQuotationRepository
    {
        Task AddAsync(Quotation quote);
        Task<IEnumerable<Quotation>> GetByOrderAsync(Guid orderId);
    }
}
