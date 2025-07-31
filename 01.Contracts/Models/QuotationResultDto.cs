using System;
using System.Collections.Generic;
using System.Text;

namespace _01.Contracts.Models
{
    public class QuotationResultDto
    {
        public Guid OrderId { get; set; }
        public string Distributor { get; set; }
        public int EstimatedDays { get; set; }
        public IEnumerable<QuotationItemResultDto> Items { get; set; }
    }
}
