using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Property
{
    public class PropertyAssignment
    {
        [Key]
        public int PropertyAssignmentID { get; set; }

        [Required]
        public int PropertyID { get; set; }
        public Property Property { get; set; }

        [Required]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; }

        [Required]
        public DateTime DateAssigned { get; set; } = DateTime.Now;

        [Required, StringLength(150)]
        public string Location { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        // New Quantity property
        [Required]
        public int Quantity { get; set; } // Tracks how many items are assigned

    }

}
