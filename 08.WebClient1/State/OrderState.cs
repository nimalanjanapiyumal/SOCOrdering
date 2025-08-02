using _01.Contracts.Models;

namespace _08.WebClient1.State
{
    public class OrderState
    {
        public Guid? OrderId { get; set; }
        public Guid? CustomerId { get; set; }
        public IEnumerable<OrderItemDto> OrderItems { get; set; } = Array.Empty<OrderItemDto>();
        public IEnumerable<QuotationResultDto> Quotations { get; set; } = Array.Empty<QuotationResultDto>();
        public IEnumerable<SelectionDto> Selections { get; set; } = Array.Empty<SelectionDto>();
        public OrderSummaryDto Summary { get; set; }
    }
}
