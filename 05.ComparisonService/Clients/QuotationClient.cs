using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
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

        public async Task<IEnumerable<QuotationResultDto>> GetQuotesAsync(Guid orderId)
        {
            var resp = await _http.GetAsync($"api/quotations/{orderId}");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get quotes for {OrderId}: {Status}", orderId, resp.StatusCode);
                return Array.Empty<QuotationResultDto>();
            }

            return await resp.Content.ReadFromJsonAsync<IEnumerable<QuotationResultDto>>();
        }
    }
}
