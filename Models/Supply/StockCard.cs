using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Supply
{
    public class StockCard
    {
        [Key]
        public int StockCardID { get; set; }

        [Required]
        [StringLength(100)]
        public string StockCardName { get; set; }

        [Required]
        [StringLength(500)]
        public string StockDescription { get; set; }

        [Required]
        [StringLength(50)]
        public string StockUnitMeasurement { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock quantity cannot be negative.")]
        public int CurrentStockQuantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.Now;

        // 🔁 Navigation property to Supplies
        public ICollection<Supply> Supplies { get; set; }

        public void UpdateStock(int quantity, decimal unitCost)
        {
            CurrentStockQuantity += quantity;
            TotalAmount = CurrentStockQuantity * unitCost;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
