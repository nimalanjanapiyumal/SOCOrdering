using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.Extensions.Logging;

namespace _02.OrderService.Clients
{
    public class ComparisonClient : IComparisonClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<ComparisonClient> _logger;

        public ComparisonClient(HttpClient http, ILogger<ComparisonClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task CompareAsync(Guid orderId)
        {
            var resp = await _http.PostAsync($"api/compare/{orderId}", null);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to trigger comparison: {Status}", resp.StatusCode);
            }
        }
    }
}
