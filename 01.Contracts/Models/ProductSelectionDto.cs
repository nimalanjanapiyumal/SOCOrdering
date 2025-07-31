using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class ProductSelectionDto
    {
        public int ProductId { get; set; }
        public string Distributor { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityChosen { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }
}
