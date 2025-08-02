using _01.Contracts.Models;

namespace _02.OrderService.Clients
{
    public class QuotationClient : IQuotationClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<QuotationClient> _logger;

        public QuotationClient(HttpClient http, ILogger<QuotationClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task RequestQuotesAsync(QuoteRequestDto request)
        {
            var resp = await _http.PostAsJsonAsync("api/quotations/request", request);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to request quotes: {Status}", resp.StatusCode);
            }
        }

        public async Task<IEnumerable<QuotationResultDto>> GetQuotesAsync(Guid orderId)
        {
            var resp = await _http.GetAsync($"api/quotations/{orderId}");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get quotes: {Status}", resp.StatusCode);
                return Array.Empty<QuotationResultDto>();
            }
            return await resp.Content.ReadFromJsonAsync<IEnumerable<QuotationResultDto>>();
        }
    }
}
