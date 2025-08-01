using _08.WebClient1;
using _08.WebClient1.Services;
using _08.WebClient1.State;
using Microsoft.AspNetCore.Components.WebAssembly.Hosting;
using Microsoft.Extensions.DependencyInjection; // Add this using directive

// Make sure you are setting the correct BaseAddress for your API services
// and that your wwwroot/index.html has the correct <app> root element.

var builder = WebAssemblyHostBuilder.CreateDefault(args);
builder.RootComponents.Add<App>("#app");

// Ensure your appsettings.json or wwwroot/appsettings.json contains the correct URLs
var serviceConfig = builder.Configuration.GetSection("Services");

// Example fallback if configuration is missing
if (string.IsNullOrEmpty(serviceConfig["OrderService"]))
{
    // Set a default or throw an error to help debugging
    throw new InvalidOperationException("OrderService URL is not configured. Check your appsettings.json or launch settings.");
}

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

// Add this to ensure each API service gets its own HttpClient with the correct BaseAddress
builder.Services.AddScoped<_08.WebClient1.Services.OrderApiService>(sp =>
{
    var config = builder.Configuration.GetSection("Services");
    var client = new HttpClient { BaseAddress = new Uri(config["OrderService"]) };
    return new _08.WebClient1.Services.OrderApiService(client);
});

await builder.Build().RunAsync();