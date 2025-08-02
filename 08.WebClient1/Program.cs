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

// HttpClient for the Blazor app itself
builder.Services.AddScoped(sp => new HttpClient { BaseAddress = new Uri(builder.HostEnvironment.BaseAddress) });

// API clients
builder.Services.AddScoped<OrderApiService>(sp =>
{
    var client = new HttpClient { BaseAddress = new Uri(serviceConfig["OrderService"]) };
    return new OrderApiService(client);
});

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
