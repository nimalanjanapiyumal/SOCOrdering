using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class QuoteRequestDto
    {
        public Guid OrderId { get; set; }
        public IEnumerable<OrderItemDto> Items { get; set; }
    }
}
