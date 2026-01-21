using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using SIETE.Models.Supply;

namespace SIETE.Models
{
    public class Supplier
    {
        [Key]
        public int SupplierID { get; set; }

        [Required(ErrorMessage = "Supplier Name is required.")]
        [StringLength(200, ErrorMessage = "Supplier Name cannot exceed 200 characters.")]
        public string SupplierName { get; set; }

        [Required(ErrorMessage = "Address is required.")]
        [StringLength(500, ErrorMessage = "Address cannot exceed 500 characters.")]
        public string Address { get; set; }

        [Phone(ErrorMessage = "Please enter a valid contact number.")]
        [StringLength(20, ErrorMessage = "Contact Number cannot exceed 20 characters.")]
        public string ContactNo { get; set; }

        [Required(ErrorMessage = "TIN is required.")]
        [StringLength(100)]
        public string TIN { get; set; }

        [Required(ErrorMessage = "VAT status is required.")]
        public bool IsVatRegistered { get; set; }

        [Required(ErrorMessage = "Date is required.")]
        [DataType(DataType.Date)]
        public DateTime Date { get; set; } = DateTime.Now;

        // Navigation property
        public virtual ICollection<SIETE.Models.Supply.Supply>? Supplies { get; set; }
    }
}
