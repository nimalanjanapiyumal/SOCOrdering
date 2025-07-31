using _01.Contracts.Models;
using System.Net.Http.Json;

namespace _08.WebClient1.Services
{
    public class NotificationApiService
    {
        private readonly HttpClient _http;

        public NotificationApiService(HttpClient http)
        {
            _http = http;
        }

        public async Task<bool> SendAsync(NotificationRequestDto request)
        {
            var resp = await _http.PostAsJsonAsync("api/notify", request);
            return resp.IsSuccessStatusCode;
        }
    }
}
