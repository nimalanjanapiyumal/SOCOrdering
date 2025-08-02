using _01.Contracts.Models;
using System.Net.Http.Json;


namespace _08.WebClient1.Services
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
