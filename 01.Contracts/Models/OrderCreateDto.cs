using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class OrderCreateDto
    {
        public Guid CustomerId { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; }
    }
}

