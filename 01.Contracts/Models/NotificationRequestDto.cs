using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class NotificationRequestDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string Email { get; set; }
        public IEnumerable<ProductSelectionDto> Selections { get; set; }
    }
}
