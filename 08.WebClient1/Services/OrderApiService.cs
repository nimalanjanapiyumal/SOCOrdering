using System.Net.Http.Json;
using System.Text.Json.Serialization;
using _01.Contracts.Models;

namespace _08.WebClient1.Services
{
    public class OrderCreatedResponseDto
    {
        [JsonPropertyName("orderId")]
        public Guid OrderId { get; set; }
    }

    public class OrderApiService
    {
        private readonly HttpClient _http;

        public OrderApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<Guid?> CreateOrderAsync(Guid customerId, OrderItemDto[] items)
        {
            var dto = new OrderCreateDto
            {
                CustomerId = customerId,
                Items = items
            };
            var resp = await _http.PostAsJsonAsync("api/orders", dto);
            if (!resp.IsSuccessStatusCode)
            {
                // Log response content for diagnostics
                var errorContent = await resp.Content.ReadAsStringAsync();
                Console.Error.WriteLine($"Order creation failed: {errorContent}");
                return null;
            }

            var payload = await resp.Content.ReadFromJsonAsync<OrderCreatedResponseDto>();
            return payload?.OrderId;
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId)
        {
            return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{orderId}");
        }

        public async Task<OrderSummaryDto> GetSummaryAsync(Guid orderId)
        {
            return await _http.GetFromJsonAsync<OrderSummaryDto>($"api/orders/{orderId}/summary");
        }

        public async Task<List<ProductDto>> GetAvailableProductsAsync()
        {
            // Example API call to backend via http
            var resp = await _http.GetAsync("api/products");
            if (!resp.IsSuccessStatusCode)
                return new List<ProductDto>();

            var products = await resp.Content.ReadFromJsonAsync<List<ProductDto>>();
            return products ?? new List<ProductDto>();
        }
    }
}
