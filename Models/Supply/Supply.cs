using SIETE.Models;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Supply
{ 
    public class Supply
    {
        [Key]
        public int SupplyID { get; set; }

        [Required]
        [ForeignKey(nameof(Supplier))]
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }

        [Required]
        [ForeignKey(nameof(FundCluster))]
        public int FundClusterID { get; set; }
        public FundCluster FundCluster { get; set; }

        // Nullable StockCardID (no need for ForeignKey attribute)
        public int? StockCardID { get; set; }  // Nullable StockCardID
        public StockCard StockCard { get; set; }

        [Required]
        [StringLength(100)]
        public string StockPropNo { get; set; } = string.Empty;

        [Required]
        [StringLength(200)]
        public string SupplyName { get; set; } = string.Empty;

        public string? Description { get; set; }

        [Required]
        public string UnitMeasurement { get; set; } = string.Empty;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        [Required]
        public DateTime DateAcquired { get; set; } = DateTime.Now;


        public enum StockCardStatus
        {
            Available = 1,
            LowStock = 2,
            OutOfStock = 3
        }

        public StockCardStatus Status { get; set; } = StockCardStatus.Available;

        public void UpdateStockStatus()
        {
            if (StockQuantity == 0)
                Status = StockCardStatus.OutOfStock;
            else if (StockQuantity <= 20)
                Status = StockCardStatus.LowStock;
            else
                Status = StockCardStatus.Available;
        }
    }
    

}
