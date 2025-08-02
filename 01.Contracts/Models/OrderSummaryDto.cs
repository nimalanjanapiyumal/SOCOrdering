using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class OrderSummaryDto
    {
        public Guid OrderId { get; set; }
        public Guid CustomerId { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
        public IEnumerable<SelectionDto> Selections { get; set; }
        public decimal TotalCost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public string SelectedVendor { get; set; }
    }
}
