using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
{
    public class NotificationClient : INotificationClient
    {
        private readonly HttpClient _http;
        private readonly ILogger<NotificationClient> _logger;

        public NotificationClient(HttpClient http, ILogger<NotificationClient> logger)
        {
            _http = http;
            _logger = logger;
        }

        public async Task SendNotificationAsync(NotificationRequestDto request)
        {
            var resp = await _http.PostAsJsonAsync("api/notify", request);
            if (!resp.IsSuccessStatusCode)
            {
                _logger.LogWarning("Failed to send notification: {Status}", resp.StatusCode);
            }
        }
    }
}
