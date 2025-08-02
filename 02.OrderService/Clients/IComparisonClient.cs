using System;
using System.Threading.Tasks;

namespace _02.OrderService.Clients
{
    public interface IComparisonClient
    {
        Task CompareAsync(Guid orderId);
    }
}
