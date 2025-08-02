using _01.Contracts.Models;

namespace _02.OrderService.Controllers
{
    public class ProductApiService
    {
        private readonly HttpClient _http;

        public ProductApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<IEnumerable<ProductDto>> GetCatalogAsync()
        {
            var resp = await _http.GetAsync("api/products");
            if (!resp.IsSuccessStatusCode) return Array.Empty<ProductDto>();
            return await resp.Content.ReadFromJsonAsync<IEnumerable<ProductDto>>();
        }

        public async Task<ProductDto> GetProductAsync(int productId)
        {
            return await _http.GetFromJsonAsync<ProductDto>($"api/products/{productId}");
        }
    }
}
