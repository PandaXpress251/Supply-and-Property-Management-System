using SIETE.Models.Supply;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Supply
{
    public class SupplyOutDetail
    {
        [Key]
        public int DetailID { get; set; }

        // Foreign key to SupplyTransaction (OUT transaction)
        [Required]
        [ForeignKey(nameof(SupplyTransaction))]
        public int TransactionID { get; set; }
        public SupplyTransaction SupplyTransaction { get; set; }

        [Required]
        [ForeignKey(nameof(Supply))]
        public int SupplyID { get; set; }
        public Supply Supply { get; set; }

        // The quantity being released or issued
        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        // Office from which the supply is being released (this is useful for tracking which office issued it)
        [Required]
        [ForeignKey(nameof(Office))]
        public int OfficeID { get; set; }
        public Office Office { get; set; }

        //// Employee responsible for receiving the supply (optional, useful for accountability)
        //public int? EmployeeID { get; set; }
        //public Employee? Employee { get; set; }
    }
}
