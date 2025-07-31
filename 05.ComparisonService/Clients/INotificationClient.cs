using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
{
    public interface INotificationClient
    {
        Task SendNotificationAsync(NotificationRequestDto request);
    }
}
