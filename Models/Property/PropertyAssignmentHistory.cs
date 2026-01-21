using SIETE.Models.Property;
using SIETE.Models;
using System.ComponentModel.DataAnnotations;

namespace SIETE.Models.Property
{
    public class PropertyAssignmentHistory
    {
        [Key]
        public int HistoryID { get; set; }

        [Required]
        public int PropertyID { get; set; }
        public Property Property { get; set; }

        public int? EmployeeID { get; set; }
        public Employee? Employee { get; set; }

        [Required]
        public DateTime StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        [Required, StringLength(150)]
        public string Location { get; set; }

        [StringLength(50)]
        public string? TPSNumber { get; set; } // Optional field to store the TPS reference

        [StringLength(50)]
        public string? TransferType { get; set; } // "Donation" for donated properties

        [StringLength(500)]
        public string? Remarks { get; set; }

        public int? PropertyTransactionID { get; set; }
        public PropertyTransaction? PropertyTransaction { get; set; }

        // New fields to track quantity changes
        [Required]
        public int PreviousQuantity { get; set; } // Quantity before the transfer

        [Required]
        public int LatestQuantity { get; set; } // Quantity after the transfer
    }

}

