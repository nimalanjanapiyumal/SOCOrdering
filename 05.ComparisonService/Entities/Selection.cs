using System.ComponentModel.DataAnnotations;

namespace _05.ComparisonService.Entities
{
    public class Selection
    {
        [Key]
        public int SelectionId { get; set; }
        public Guid OrderId { get; set; }
        public int ProductId { get; set; }
        public string Distributor { get; set; }
        public decimal UnitPrice { get; set; }
        public int QuantityChosen { get; set; }
    }
}
