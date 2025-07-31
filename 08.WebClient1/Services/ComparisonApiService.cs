using System.Net.Http.Json;

namespace _08.WebClient1.Services
{
    public class ComparisonApiService
    {
        private readonly HttpClient _http;

        public ComparisonApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<object> CompareAsync(Guid orderId)
        {
            var resp = await _http.PostAsync($"api/compare/{orderId}", null);
            if (!resp.IsSuccessStatusCode) return null;
            return await resp.Content.ReadFromJsonAsync<object>();
        }
    }
}
