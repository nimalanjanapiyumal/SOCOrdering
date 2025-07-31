using System;

namespace _07.ClientApp
{
    internal class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public System.Collections.Generic.List<OrderItemDto> Items { get; set; }
    }
}