using _05.ComparisonService.Clients;
using _05.ComparisonService.Repositories;
using _05.ComparisonService.Data; // Add this using directive
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// DB
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

// Repositories
builder.Services.AddScoped<ISelectionRepository, SelectionRepository>();

// HTTP clients to other services (adjust ports to your running OrderService/QuotationService)
builder.Services.AddHttpClient<IOrderClient, OrderClient>(client =>
    client.BaseAddress = new Uri("https://localhost:5001/")); // OrderService

builder.Services.AddHttpClient<IQuotationClient, QuotationClient>(client =>
    client.BaseAddress = new Uri("https://localhost:5002/")); // QuotationService
builder.Services.AddHttpClient<INotificationClient, NotificationClient>(client =>
{
    client.BaseAddress = new Uri("https://localhost:5003/");
})
.ConfigurePrimaryHttpMessageHandler(() => new HttpClientHandler
{
    ServerCertificateCustomValidationCallback = HttpClientHandler.DangerousAcceptAnyServerCertificateValidator
});



builder.Services.AddControllers();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

var app = builder.Build();
if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
app.UseHttpsRedirection();
app.UseAuthorization();
app.MapControllers();
app.Run();