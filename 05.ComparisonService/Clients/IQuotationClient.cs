using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
{
    public interface IQuotationClient
    {
        Task<IEnumerable<QuotationResultDto>> GetQuotesAsync(Guid orderId);
    }
}
