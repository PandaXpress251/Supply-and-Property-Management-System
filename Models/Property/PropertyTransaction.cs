using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace SIETE.Models.Property
{
    public enum TransactionTypes
    {
        Aquired = 1,
        Transferred = 2,
        ReTransferred = 3,
        Disposed = 4
    }

    public class PropertyTransaction
    {
        [Key]
        public int TransactionID { get; set; }

        [Required]
        public TransactionTypes Type { get; set; } // e.g., "Received", "Transferred", "Disposed"

        [Required]
        public DateTime TransactionDate { get; set; } = DateTime.Now;

        //// Optional: the person who performed the transaction
        //public int? PerformedByID { get; set; }
        //public Employee? PerformedBy { get; set; }

        [StringLength(500)]
        public string? Remarks { get; set; }

        // Navigation property

        public ICollection<PropertyTransactionDetail> PropertyTransactionDetails { get; set; } = new List<PropertyTransactionDetail>();
    }

}
