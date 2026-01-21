using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using SIETE.Models.Supply;

namespace SIETE.Models
{
    public enum Roles
    {
        Admin = 1,
        User = 2
    }

    public class UserAccount
    {
        [Key]
        public int UserID { get; set; }

        [Required]
        [ForeignKey(nameof(Employee))]
        public int EmployeeID { get; set; }
        public Employee Employee { get; set; } // Not Nullable, the account must be linked to an employee

        [Required]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [Required]
        public string Password { get; set; } = string.Empty;

        [Required]
        public Roles Role { get; set; }

     
    }

}
