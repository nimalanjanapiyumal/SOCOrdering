using System.ComponentModel.DataAnnotations;

namespace _03.QuotationService.Entities
{
    public class Quotation
    {
        [Key]
        public int QuoteId { get; set; }
        public Guid OrderId { get; set; }
        public string Distributor { get; set; }
        public int EstimatedDays { get; set; }
        public ICollection<QuotationItem> Items { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
