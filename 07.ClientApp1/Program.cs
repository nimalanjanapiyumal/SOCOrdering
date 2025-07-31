using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace _07.ClientApp1
{
    internal class Program
    {
        private static readonly HttpClient OrderClient = new HttpClient { BaseAddress = new Uri("https://localhost:5001/") };
        private static readonly HttpClient QuotationClient = new HttpClient { BaseAddress = new Uri("https://localhost:5002/") };
        private static readonly HttpClient ComparisonClient = new HttpClient { BaseAddress = new Uri("https://localhost:5004/") }; // adjust port if different
        private static readonly HttpClient NotificationClient = new HttpClient { BaseAddress = new Uri("https://localhost:5003/") };

        private static async Task Main()
        {
            Console.WriteLine("=== Gadget Hub Ordering Flow ===");

            // 1. Create order
            var orderCreate = new OrderCreateDto
            {
                CustomerId = Guid.NewGuid(),
                Items = new[]
                {
                new OrderItemDto { ProductId = 1, Quantity = 2 },
                new OrderItemDto { ProductId = 2, Quantity = 1 }
            }
            };

            var orderResp = await OrderClient.PostAsJsonAsync("api/orders", orderCreate);
            if (!orderResp.IsSuccessStatusCode)
            {
                Console.WriteLine("Failed to create order: " + orderResp.StatusCode);
                return;
            }

            var created = await orderResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
            Guid orderId = Guid.Parse(created["orderId"].ToString());
            Console.WriteLine($"Order created with ID: {orderId}");

            // 2. Wait a bit for quotation service to populate (could poll the quotation endpoint)
            IEnumerable<QuotationResultDto> quotes = null;
            for (int attempt = 1; attempt <= 5; attempt++)
            {
                Console.WriteLine($"Polling for quotations (attempt {attempt})...");
                var quoteResp = await QuotationClient.GetAsync($"api/quotations/{orderId}");
                if (quoteResp.IsSuccessStatusCode)
                {
                    quotes = await quoteResp.Content.ReadFromJsonAsync<IEnumerable<QuotationResultDto>>();
                    if (quotes != null && quotes.Any())
                        break;
                }

                await Task.Delay(1500 * attempt); // backoff
            }

            if (quotes == null || !quotes.Any())
            {
                Console.WriteLine("No quotations available. Exiting.");
                return;
            }

            Console.WriteLine("Received quotations:");
            foreach (var q in quotes)
            {
                Console.WriteLine($"- Distributor: {q.Distributor}, ETA: {q.EstimatedDays} days");
                foreach (var item in q.Items)
                {
                    Console.WriteLine($"   Product {item.ProductId}: Price {item.UnitPrice:C}, Available {item.Available}");
                }
            }

            // 3. Trigger comparison
            var compareResp = await ComparisonClient.PostAsync($"api/compare/{orderId}", null);
            if (!compareResp.IsSuccessStatusCode)
            {
                Console.WriteLine("Comparison failed: " + compareResp.StatusCode);
                return;
            }

            var compareResult = await compareResp.Content.ReadFromJsonAsync<dynamic>();
            Console.WriteLine("Comparison result selections:");
            // Assuming the response shape has Selections
            foreach (var sel in compareResult.selections)
            {
                Console.WriteLine($"Product {sel.productId}: Distributor {sel.distributor}, UnitPrice {sel.unitPrice}, Qty {sel.quantityChosen}");
            }

            // 4. Build notification request
            var selectionDtos = new List<ProductSelectionDto>();
            foreach (var sel in compareResult.selections)
            {
                selectionDtos.Add(new ProductSelectionDto
                {
                    ProductId = (int)sel.productId,
                    Distributor = (string)sel.distributor,
                    UnitPrice = Convert.ToDecimal(sel.unitPrice),
                    QuantityChosen = (int)sel.quantityChosen,
                    EstimatedDeliveryDays = 0 // could be enhanced if available
                });
            }

            var notification = new NotificationRequestDto
            {
                OrderId = orderId,
                CustomerId = orderCreate.CustomerId,
                Email = "customer@example.com",
                Selections = selectionDtos
            };

            var notifyResp = await NotificationClient.PostAsJsonAsync("api/notify", notification);
            if (notifyResp.IsSuccessStatusCode)
            {
                Console.WriteLine("Notification sent successfully.");
            }
            else
            {
                Console.WriteLine("Notification failed: " + notifyResp.StatusCode);
            }

            // 5. Fetch final summary (assuming endpoint exists)
            var summaryResp = await OrderClient.GetAsync($"api/orders/{orderId}/summary");
            if (summaryResp.IsSuccessStatusCode)
            {
                var summary = await summaryResp.Content.ReadFromJsonAsync<OrderSummaryDto>();
                Console.WriteLine("\n=== Final Order Summary ===");
                Console.WriteLine($"OrderId: {summary.OrderId}");
                Console.WriteLine($"CustomerId: {summary.CustomerId}");
                Console.WriteLine($"Status: {summary.Status}");
                Console.WriteLine($"TotalCost: {summary.TotalCost:C}");
                Console.WriteLine($"EstimatedDeliveryDays: {summary.EstimatedDeliveryDays}");
                foreach (var s in summary.Selections)
                {
                    Console.WriteLine($"- Product {s.ProductId}: {s.Distributor} x{s.QuantityChosen} @ {s.UnitPrice:C}");
                }
            }
            else
            {
                Console.WriteLine("Failed to get order summary.");
            }
        }
    }

    internal class OrderItemDto
    {
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }

    internal class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public OrderItemDto[] Items { get; set; }
    }
}
