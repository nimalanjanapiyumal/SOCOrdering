using _01.Contracts.Models;

namespace _06.NotificationService
{
    public interface INotificationSender
    {
        Task SendAsync(NotificationRequestDto request);
    }
}
