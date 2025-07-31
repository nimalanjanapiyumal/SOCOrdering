using System;
using System.Collections.Generic;
using System.Text;
using System.Threading.Tasks;
using _01.Contracts.Models;

namespace _01.Contracts.Repositories
{
    public interface IOrderRepository
    {
        Task<Guid> CreateOrderAsync(Guid customerId, IEnumerable<OrderItemDto> items);
        Task<OrderDto> GetOrderAsync(Guid orderId);

        Task UpdateStatusAsync(Guid orderId, string status);
    }
}
