namespace _02.OrderService.Entities
{
    public class OrderItem
    {
        public int OrderItemId { get; set; }
        public Guid OrderId { get; set; }
        public Order Order { get; set; }
        public int ProductId { get; set; }
        public int Quantity { get; set; }
    }
}
