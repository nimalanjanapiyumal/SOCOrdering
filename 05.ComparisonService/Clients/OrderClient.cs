using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
{
    public class OrderClient : IOrderClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<OrderClient> _logger;

        public OrderClient(HttpClient http, ILogger<OrderClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId)
        {
            var resp = await _http.GetAsync($"api/orders/{orderId}");
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to get order {OrderId}: {Status}", orderId, resp.StatusCode);
                return null;
            }
            return await resp.Content.ReadFromJsonAsync<OrderDto>();
        }
    }
}
