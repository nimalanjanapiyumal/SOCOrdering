using _01.Contracts.Models;

namespace _06.NotificationService
{
    public class ConsoleNotificationSender : INotificationSender
    {
        private readonly ILogger<ConsoleNotificationSender> _logger;

        public ConsoleNotificationSender(ILogger<ConsoleNotificationSender> logger)
        {
            _logger = logger;
        }

        public Task SendAsync(NotificationRequestDto request)
        {
            var header = $"Notification for Order {request.OrderId} (Customer: {request.CustomerId}, Email: {request.Email})";
            _logger.LogInformation(header);
            foreach (var sel in request.Selections)
            {
                _logger.LogInformation(
                    "- Product {ProductId}: Qty {Qty} from {Distributor} @ {Price:C}, ETA {ETA} days",
                    sel.ProductId, sel.QuantityChosen, sel.Distributor, sel.UnitPrice, sel.EstimatedDeliveryDays);
            }

            _logger.LogInformation("Estimated overall delivery: {MaxEta} days",
                request.Selections.Any() ? request.Selections.Max(s => s.EstimatedDeliveryDays) : 0);

            // In real system, replace with email/SMS sender here.
            return Task.CompletedTask;
        }
    }
}
