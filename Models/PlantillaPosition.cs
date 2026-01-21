using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIETE.Models
{
    public class PlantillaPosition
    {
        [Key]
        public int PositionID { get; set; }
        public override string ToString() => PositionTitle;

        [Required]
        [StringLength(100)]
        public string PositionTitle { get; set; } // ex. "Supply Officer III"

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; } // Default to true for new positions

        // Navigation property to related Employees
        public ICollection<Employee>? Employees { get; set; }
    }
}
