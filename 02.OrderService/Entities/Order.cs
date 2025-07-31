namespace _02.OrderService.Entities
{
    public class Order
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public ICollection<OrderItem>? Items { get; set; }
    }
}
