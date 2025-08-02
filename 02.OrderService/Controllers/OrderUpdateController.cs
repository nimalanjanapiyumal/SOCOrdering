using _01.Contracts.Models;
using _01.Contracts.Repositories;
using Microsoft.AspNetCore.Mvc;

namespace _02.OrderService.Controllers
{
    [ApiController]
    [Route("api/orders")]
    public class OrderUpdateController : ControllerBase
    {
        private readonly IOrderRepository _repo;
        private readonly ILogger<OrderUpdateController> _logger;

        public OrderUpdateController(IOrderRepository repo, ILogger<OrderUpdateController> logger)
        {
            _repo = repo;
            _logger = logger;
        }

        // Endpoint used by ComparisonService to push final selection and mark order completed
        [HttpPost("{orderId:guid}/finalize")]
        public async Task<IActionResult> FinalizeOrder(Guid orderId, [FromBody] OrderSummaryDto summary)
        {
            var existing = await _repo.GetOrderAsync(orderId);
            if (existing == null) return NotFound();

            // In a fuller design you might persist selections; here we update status
            // (Extend repository to store selections if desired)
            // For now, update order status via a new method (you may need to add it)
            // Assume repository exposes UpdateStatusAsync:
            if (_repo is not null && summary != null)
            {
                // You need to implement UpdateStatusAsync in IOrderRepository and its implementation
                await ((dynamic)_repo).UpdateStatusAsync(orderId, "Completed");
            }

            _logger.LogInformation("Order {OrderId} finalized with status Completed.", orderId);
            return Ok(new { message = "Order finalized" });
        }

        [HttpGet("{orderId:guid}/summary")]
        public async Task<IActionResult> GetSummary(Guid orderId)
        {
            var order = await _repo.GetOrderAsync(orderId);
            if (order == null) return NotFound();

            // Fetch selections from ComparisonService
            // You could inject an HTTP client for ComparisonService here; for brevity assume caller aggregates externally.
            // Placeholder response:
            var summary = new OrderSummaryDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Selections = Array.Empty<SelectionDto>(),
                TotalCost = 0,
                EstimatedDeliveryDays = 0,
                SelectedVendor = string.Empty
            };
            return Ok(summary);
        }
    }
}
