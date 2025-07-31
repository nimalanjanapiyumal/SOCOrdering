using System.ComponentModel.DataAnnotations;

namespace _03.QuotationService.Entities
{
    public class QuotationItem
    {
        [Key]
        public int QuotationItemId { get; set; }
        public int QuotationId { get; set; }
        public Quotation Quotation { get; set; }
        public int ProductId { get; set; }
        public decimal UnitPrice { get; set; }
        public int Available { get; set; }
    }
}
