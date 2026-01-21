using SIETE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Supply
{
    public enum TransactionType
    {
        IN = 1,  
        OUT = 2  
    }

    public class SupplyTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public TransactionType Type { get; set; }

        [Required]
        public DateTime Date { get; set; } = DateTime.UtcNow;

        public string? Remarks { get; set; }

        // Navigation properties for related transaction details
        public ICollection<SupplyInDetail> SupplyInDetails { get; set; } = new List<SupplyInDetail>(); // For IN transactions
        public ICollection<SupplyOutDetail> SupplyOutDetails { get; set; } = new List<SupplyOutDetail>(); // For OUT transactions


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

    }
}
