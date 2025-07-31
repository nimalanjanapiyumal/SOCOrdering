using _08.WebClient1;
using _08.WebClient1.Services;
using _08.WebClient1.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection; // Add this using directive

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Load service URLs
var serviceConfig = builder.Configuration.GetSection("Services");

// HttpClients
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(serviceConfig["OrderService"]) });
builder.Services.AddScoped<_08.WebClient1.Services.OrderApiService>(); // uses OrderService client internally

builder.Services.AddScoped<QuotationApiService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(serviceConfig["QuotationService"]) };
    return new QuotationApiService(client);
});
builder.Services.AddScoped<ComparisonApiService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(serviceConfig["ComparisonService"]) };
    return new ComparisonApiService(client);
});
builder.Services.AddScoped<NotificationApiService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(serviceConfig["NotificationService"]) };
    return new NotificationApiService(client);
});

// Shared state
builder.Services.AddSingleton<OrderState>();

await builder.Build().RunAsync();