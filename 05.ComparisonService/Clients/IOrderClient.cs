using _01.Contracts.Models;

namespace _05.ComparisonService.Clients
{
    public interface IOrderClient
    {
        Task<OrderDto> GetOrderAsync(Guid orderId);
    }
}
