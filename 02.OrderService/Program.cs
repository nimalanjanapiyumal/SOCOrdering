using _01.Contracts.Messaging;
using _01.Contracts.Repositories;
using _02.OrderService.Clients;
using _02.OrderService.Controllers;
using _02.OrderService.Data;
using _02.OrderService.Messaging;
using _02.OrderService.Repositories;
using Microsoft.EntityFrameworkCore;

namespace _02.OrderService
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // DbContext
            builder.Services.AddDbContext<ApplicationDbContext>(options =>
                options.UseSqlServer(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.AddHttpClient<IQuotationClient, QuotationClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5002/"); // Use http if QuotationService is not running with SSL
            });

            builder.Services.AddScoped<ProductApiService>(sp =>
                new ProductApiService(new HttpClient { BaseAddress = new Uri("https://localhost:5001/") })); // same host as OrderService

            // Repository
            builder.Services.AddScoped<IOrderRepository, OrderRepository>();

            // Message bus (in-memory stub)
            builder.Services.AddSingleton<IMessageBus, InMemoryMessageBus>();

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();
            builder.Services.AddSwaggerGen();

            var app = builder.Build();
            if (app.Environment.IsDevelopment()) { app.UseSwagger(); app.UseSwaggerUI(); }
            app.UseHttpsRedirection();
            app.UseAuthorization();
            app.MapControllers();
            app.Run();
        }
    }
}