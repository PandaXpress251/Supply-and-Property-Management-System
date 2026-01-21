using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Property
{
    public class PropertyCard
    {
        [Key]
        public int PropertyCardID { get; set; }

        [Required, StringLength(200)]
        public string PropertyCardName { get; set; }

        [Required, StringLength(500)]
        public string PropertyDescription { get; set; }

        [Required, StringLength(50)]
        public string PropertyUnitMeasurement { get; set; }

        [Required]
        public int CurrentStockQuantity { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        [Required]
        public DateTime LastUpdated { get; set; } = DateTime.UtcNow;

        public ICollection<Property> Properties { get; set; } = new List<Property>();

        public void UpdateQuantity(int quantity, decimal unitCost)
        {
            CurrentStockQuantity += quantity;
            TotalAmount = CurrentStockQuantity * unitCost;
            LastUpdated = DateTime.UtcNow;
        }
    }
}
