using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class ProductDto
    {
        public int ProductId { get; set; }           // internal SKU/ID
        public string Name { get; set; }
        public string Description { get; set; }
        public decimal BasePrice { get; set; }      // for display (could be ignored if quote-driven)
        public int MaxOrderQuantity { get; set; }   // front-end limit
    }
}
