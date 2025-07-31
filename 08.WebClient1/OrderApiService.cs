using _01.Contracts.Models;
using System.Net.Http.Json;

namespace _08.WebClient1
{
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
        if (!resp.IsSuccessStatusCode) return null;

        var payload = await resp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
        if (payload != null && payload.TryGetValue("orderId", out var oid))
        {
            return Guid.Parse(oid.ToString());
        }
        return null;
    }

    public async Task<OrderDto> GetOrderAsync(Guid orderId)
    {
        return await _http.GetFromJsonAsync<OrderDto>($"api/orders/{orderId}");
    }
}
}
