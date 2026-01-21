using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Property
{
    public class Property
    {
        [Key]
        public int PropertyID { get; set; }

        // Foreign key to Supplier
        [Required]
        public int SupplierID { get; set; }
        public Supplier Supplier { get; set; }

        // Foreign key to FundCluster
        [Required]
        public int FundClusterID { get; set; }
        public FundCluster FundCluster { get; set; }
        public int? PropertyCardID { get; set; }
        public PropertyCard PropertyCard { get; set; }

        // Property specific information
        [Required]
        [StringLength(100)]
        public string StockPropNo { get; set; }

        [Required]
        [StringLength(200)]
        public string PropertyName { get; set; }

        public string? Description { get; set; }
        public string? UnitMeasurement { get; set; }

        [Required]
        [Range(0, int.MaxValue, ErrorMessage = "Stock Quantity cannot be negative.")]
        public int StockQuantity { get; set; }

        // Cost and acquisition info
        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        public DateTime DateAcquired { get; set; } = DateTime.UtcNow;

        // Property status (e.g., Active, Disposed)
        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Active";  // Default status is Active

        // Category of the property based on price thresholds
        [Required]
        [StringLength(50)]
        public string Category { get; set; }

        // Navigation properties
        public ICollection<PropertyAssignmentHistory> PropertyAssignmentHistory { get; set; } = new List<PropertyAssignmentHistory>();

        // Update category dynamically based on thresholds
        public void UpdateCategory(PriceThreshold threshold)
        {
            // Logic to determine the category based on unit cost and the thresholds
            if (UnitCost >= threshold.PropertyThreshold)
            {
                Category = "Property";  // Low-value property
            }
            else if (UnitCost < threshold.SPThreshold)
            {
                Category = "Low-Valued SP";  // Low-valued special property
            }
            else
            {
                Category = "High-Valued SP";  // High-value special property
            }
        }
    }
}
