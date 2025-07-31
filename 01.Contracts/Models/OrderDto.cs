using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class OrderDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public DateTime CreatedAt { get; set; }
        public string Status { get; set; }
        public IEnumerable<OrderItemDto>? Items { get; set; }
    }
}

