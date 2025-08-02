using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class QuotationItemResultDto
    {
        public int QuotationItemId { get; set; }
        public int QuotationId { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Available { get; set; }
    }
}