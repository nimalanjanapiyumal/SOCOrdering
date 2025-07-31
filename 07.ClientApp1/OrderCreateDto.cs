using System;

namespace _07.ClientApp1
{
    internal class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public OrderItemDto[] Items { get; set; }
    }
}