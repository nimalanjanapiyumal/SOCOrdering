using _01.Contracts.Models;

namespace _02.OrderService.Clients
{
    public interface IQuotationClient
    {
        Task RequestQuotesAsync(QuoteRequestDto request);
        Task<IEnumerable<QuotationResultDto>> GetQuotesAsync(Guid orderId);
    }
}
