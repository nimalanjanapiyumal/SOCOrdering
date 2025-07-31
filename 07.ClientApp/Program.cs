using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading.Tasks;
using _01.Contracts.Models;


namespace _07.ClientApp
{
    internal class Program
    {
        // Adjust these to the actual ports your services are running on.
        private static readonly string OrderServiceBase = "https://localhost:5001/";
        private static readonly string QuotationServiceBase = "https://localhost:5002/";
        private static readonly string ComparisonServiceBase = "https://localhost:5004/";
        private static readonly string NotificationServiceBase = "https://localhost:5003/";

        static async Task Main()
        {
            try
            {
                var httpOrder = new HttpClient { BaseAddress = new Uri(OrderServiceBase) };
                var httpQuotation = new HttpClient { BaseAddress = new Uri(QuotationServiceBase) };
                var httpCompare = new HttpClient { BaseAddress = new Uri(ComparisonServiceBase) };
                var httpNotify = new HttpClient { BaseAddress = new Uri(NotificationServiceBase) };

                // 1. Create Order
                var orderCreate = new OrderCreateDto
                {
                    CustomerId = Guid.NewGuid(),
                    Items = new List<OrderItemDto>
                    {
                        new OrderItemDto { ProductId = 1, Quantity = 2 },
                        new OrderItemDto { ProductId = 2, Quantity = 1 }
                    }
                };

                Console.WriteLine("Placing order...");
                var orderResp = await httpOrder.PostAsJsonAsync("api/orders", orderCreate);
                if (!orderResp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Order creation failed: {orderResp.StatusCode}");
                    return;
                }

                var createdObj = await orderResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                if (createdObj == null || !createdObj.TryGetValue("orderId", out var orderIdRaw))
                {
                    Console.WriteLine("Unexpected response when creating order.");
                    return;
                }

                if (!Guid.TryParse(orderIdRaw.ToString(), out var orderId))
                {
                    Console.WriteLine("Failed to parse orderId.");
                    return;
                }

                Console.WriteLine($"Order created: {orderId}");

                // 2. Wait for quotations to be generated
                await Task.Delay(2000);

                // 3. Fetch quotations
                Console.WriteLine("Fetching quotations...");
                var quotesResp = await httpQuotation.GetAsync($"api/quotations/{orderId}");
                if (!quotesResp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Failed to get quotations: {quotesResp.StatusCode}");
                    return;
                }

                var quotations = await quotesResp.Content.ReadFromJsonAsync<IEnumerable<QuotationResultDto>>();
                if (quotations == null)
                {
                    Console.WriteLine("No quotations returned.");
                    return;
                }

                Console.WriteLine("Received quotations:");
                foreach (var q in quotations)
                {
                    Console.WriteLine($"- Distributor: {q.Distributor}, ETA: {q.EstimatedDays} days");
                    foreach (var item in q.Items)
                    {
                        Console.WriteLine($"   Product {item.ProductId}: Price {item.UnitPrice}, Available {item.Available}");
                    }
                }

                // 4. Trigger comparison
                Console.WriteLine("Triggering comparison...");
                var compareResp = await httpCompare.PostAsync($"api/compare/{orderId}", null);
                if (!compareResp.IsSuccessStatusCode)
                {
                    Console.WriteLine($"Comparison failed: {compareResp.StatusCode}");
                    return;
                }

                var comparisonResult = await compareResp.Content.ReadFromJsonAsync<Dictionary<string, object>>();
                Console.WriteLine("Comparison result:");
                if (comparisonResult != null && comparisonResult.TryGetValue("Selections", out var selectionsObj))
                {
                    Console.WriteLine(selectionsObj);
                }

                // 5. Fetch finalized order summary
                Console.WriteLine("Fetching order summary...");
                var summaryResp = await httpOrder.GetAsync($"api/orders/{orderId}/summary");
                if (summaryResp.IsSuccessStatusCode)
                {
                    var summary = await summaryResp.Content.ReadFromJsonAsync<OrderSummaryDto>();
                    if (summary != null)
                    {
                        Console.WriteLine($"Order Status: {summary.Status}");
                        Console.WriteLine($"Total Cost: {summary.TotalCost}");
                        Console.WriteLine("Selections:");
                        foreach (var sel in summary.Selections)
                        {
                            Console.WriteLine($"- Product {sel.ProductId} from {sel.Distributor}: qty {sel.QuantityChosen} @ {sel.UnitPrice}");
                        }
                    }
                }
                else
                {
                    Console.WriteLine("Could not retrieve summary.");
                }

                Console.WriteLine("Flow complete. Check NotificationService logs for customer notification.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Unhandled exception: {ex.Message}");
            }
        }
    }
}

