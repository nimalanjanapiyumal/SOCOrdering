using _01.Contracts.Models;
using System.Net.Http.Json;

namespace _08.WebClient1.Services
{
    public class QuotationApiService
    {
        private readonly HttpClient _http;

        public QuotationApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<QuotationResultDto>> GetQuotesAsync(Guid orderId)
        {
            var resp = await _http.GetAsync($"api/quotations/{orderId}");
            if (!resp.IsSuccessStatusCode) return Array.Empty<QuotationResultDto>();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<QuotationResultDto>>();
        }
    }
}
