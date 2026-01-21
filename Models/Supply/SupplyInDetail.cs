using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Supply
{
    public class SupplyInDetail
    {
        [Key]
        public int DetailID { get; set; }

        [Required]
        [ForeignKey(nameof(SupplyTransaction))]
        public int TransactionID { get; set; }
        public SupplyTransaction SupplyTransaction { get; set; }

        [Required]
        [ForeignKey(nameof(Supply))]
        public int SupplyID { get; set; }
        public Supply Supply { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; } // Stored in database
    }
}