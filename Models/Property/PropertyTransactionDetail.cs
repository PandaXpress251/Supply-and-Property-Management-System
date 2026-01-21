using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models.Property
{
    public class PropertyTransactionDetail
    {
        [Key]
        public int PropertyTransactionDetailID { get; set; }

        [Required]
        [ForeignKey(nameof(PropertyTransaction))]
        public int TransactionID { get; set; }
        public PropertyTransaction PropertyTransaction { get; set; }

        [Required]
        [ForeignKey(nameof(Property))]
        public int PropertyID { get; set; }
        public Property Property { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal UnitCost { get; set; }

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }

        // Used in case of Transfer or Disposal
        public int? EmployeeID { get; set; }
        public Employee? Employee { get; set; }

        [StringLength(200)]
        public string? Location { get; set; }

        // Used for disposal type selection
        [StringLength(50)]
        public string? DisposalType { get; set; }

        // Used for assignment disposal
        public int? AssignmentID { get; set; }
    }
}
