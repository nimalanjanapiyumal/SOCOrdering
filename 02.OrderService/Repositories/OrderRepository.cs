using _02.OrderService.Entities;
using _01.Contracts.Models;
using _01.Contracts.Repositories;
using _02.OrderService.Data;
using Microsoft.EntityFrameworkCore;

namespace _02.OrderService.Repositories
{
    public class OrderRepository : IOrderRepository
    {
        private readonly ApplicationDbContext _db;
        public OrderRepository(ApplicationDbContext db) => _db = db;

        public async Task<Guid> CreateOrderAsync(Guid customerId, IEnumerable<OrderItemDto> items)
        {
            var order = new Order
            {
                OrderId = Guid.NewGuid(),
                CustomerId = customerId,
                CreatedAt = DateTime.UtcNow,
                Status = "PendingQuotes",
                Items = items.Select(i => new OrderItem
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                }).ToList()
            };
            _db.Orders.Add(order);
            await _db.SaveChangesAsync();
            return order.OrderId;
        }

        public async Task<OrderDto> GetOrderAsync(Guid orderId)
        {
            var order = await _db.Orders
                .Include(o => o.Items) // requires Microsoft.EntityFrameworkCore
                .FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order == null) return null;

            return new OrderDto
            {
                OrderId = order.OrderId,
                CustomerId = order.CustomerId,
                CreatedAt = order.CreatedAt,
                Status = order.Status,
                Items = order.Items.Select(i => new OrderItemDto
                {
                    ProductId = i.ProductId,
                    Quantity = i.Quantity
                })
            };
        }

        public async Task UpdateStatusAsync(Guid orderId, string status)
        {
            var order = await _db.Orders.FirstOrDefaultAsync(o => o.OrderId == orderId);
            if (order != null)
            {
                order.Status = status;
                await _db.SaveChangesAsync();
            }
        }
    }
}