using _01.Contracts.Models;
using _05.ComparisonService.Clients;
using _05.ComparisonService.Entities;
using _05.ComparisonService.Repositories;
using Microsoft.AspNetCore.Mvc;
using System.Net.Http;
using System.Net.Http.Json;

namespace _05.ComparisonService.Controllers
{
    [ApiController]
    [Route("api/compare")]
    public class CompareController : ControllerBase
    {
        private readonly IOrderClient _orderClient;
        private readonly IQuotationClient _quotationClient;
        private readonly ISelectionRepository _selectionRepo;
        private readonly ILogger<CompareController> _logger;
        private readonly INotificationClient _notificationClient;
        private readonly HttpClient _httpClient;

        public CompareController(
            IOrderClient orderClient,
            IQuotationClient quotationClient,
            ISelectionRepository selectionRepo,
            ILogger<CompareController> logger,
            INotificationClient notificationClient,
            HttpClient httpClient)
        {
            _orderClient = orderClient;
            _quotationClient = quotationClient;
            _selectionRepo = selectionRepo;
            _logger = logger;
            _notificationClient = notificationClient;
            _httpClient = httpClient;
        }

        [HttpPost("{orderId:guid}")]
        public async Task<IActionResult> Compare(Guid orderId)
        {
            var order = await _orderClient.GetOrderAsync(orderId);
            if (order == null) return NotFound("Order not found.");

            var quotes = await _quotationClient.GetQuotesAsync(orderId);
            if (!quotes.Any()) return BadRequest("No quotations available.");

            var selections = new List<Selection>();

            foreach (var item in order.Items)
            {
                // For each product, pick quote with lowest unit price that has sufficient availability
                var candidateQuotes = quotes
                    .SelectMany(q => q.Items.Select(i => new
                    {
                        q.Distributor,
                        q.EstimatedDays,
                        Item = i
                    }))
                    .Where(x => x.Item.ProductId == item.ProductId && x.Item.Available >= item.Quantity)
                    .OrderBy(x => x.Item.UnitPrice)
                    .ToList();

                if (candidateQuotes.Any())
                {
                    var best = candidateQuotes.First();
                    selections.Add(new Selection
                    {
                        OrderId = orderId,
                        ProductId = item.ProductId,
                        Distributor = best.Distributor,
                        UnitPrice = best.Item.UnitPrice,
                        QuantityChosen = item.Quantity
                    });
                }
                else
                {
                    // Fallback: split across multiple distributors if no single has full availability
                    var partials = quotes
                        .SelectMany(q => q.Items.Select(i => new
                        {
                            q.Distributor,
                            Item = i
                        }))
                        .Where(x => x.Item.ProductId == item.ProductId && x.Item.Available > 0)
                        .OrderBy(x => x.Item.UnitPrice)
                        .ToList();

                    int remaining = item.Quantity;
                    foreach (var part in partials)
                    {
                        int take = Math.Min(remaining, part.Item.Available);
                        selections.Add(new Selection
                        {
                            OrderId = orderId,
                            ProductId = item.ProductId,
                            Distributor = part.Distributor,
                            UnitPrice = part.Item.UnitPrice,
                            QuantityChosen = take
                        });
                        remaining -= take;
                        if (remaining == 0) break;
                    }

                    if (remaining > 0)
                    {
                        // insufficient total availability
                        return BadRequest($"Cannot fulfill product {item.ProductId}: shortage of {remaining} units.");
                    }
                }
            }

            // Persist selections
            await _selectionRepo.AddRangeAsync(selections);

            // after persisting selections
            var selectionDtos = selections.Select(s => new SelectionDto
            {
                ProductId = s.ProductId,
                Distributor = s.Distributor,
                UnitPrice = s.UnitPrice,
                QuantityChosen = s.QuantityChosen
            }).ToList();

            var totalCost = selectionDtos.Sum(s => s.UnitPrice * s.QuantityChosen);
            var summary = new OrderSummaryDto
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = "Completed",
                Selections = selectionDtos,
                TotalCost = totalCost,
                EstimatedDeliveryDays = 0, // could compute max ETA if available
                SelectedVendor = selectionDtos
                    .GroupBy(s => s.Distributor)
                    .OrderByDescending(g => g.Sum(x => x.QuantityChosen))
                    .First().Key
            };

            // after saving selections
            var notificationSelections = selections.Select(s => new ProductSelectionDto
            {
                ProductId = s.ProductId,
                Distributor = s.Distributor,
                UnitPrice = s.UnitPrice,
                QuantityChosen = s.QuantityChosen,
                // You need to supply EstimatedDeliveryDays—if you captured it earlier, include it; else default to a value
                EstimatedDeliveryDays = (int?)null ?? 0
            }).ToList();

            var notification = new NotificationRequestDto
            {
                OrderId = orderId,
                CustomerId = order.CustomerId,
                Email = "customer@example.com", // replace with real customer email when available
                Selections = notificationSelections
            };

            var orderServiceResponse = await _httpClient.PostAsJsonAsync($"api/orders/{orderId}/finalize", summary);

            await _notificationClient.SendNotificationAsync(notification);


            return Ok(new
            {
                OrderId = orderId,
                Selections = selections.Select(s => new
                {
                    s.ProductId,
                    s.Distributor,
                    s.UnitPrice,
                    s.QuantityChosen
                })
            });
        }

        [HttpGet("{orderId:guid}")]
        public async Task<IActionResult> GetSelections(Guid orderId)
        {
            var stored = await _selectionRepo.GetByOrderAsync(orderId);
            return Ok(stored);
        }
    }
}
