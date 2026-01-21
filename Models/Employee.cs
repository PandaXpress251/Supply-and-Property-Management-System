using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel; // For DefaultValue

namespace SIETE.Models
{
    public class Employee
    {
        [Key]
        public int EmployeeID { get; set; }

        [StringLength(20)]
        [Display(Name = "Title")]
        public string? Title { get; set; } // Atty., Engr., Dr., etc.

        [Required]
        [StringLength(50)]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }

        [Required]
        [StringLength(50)]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }

        [StringLength(10)]
        [Display(Name = "Suffix")]
        public string? Suffix { get; set; } // Jr., Sr., III, etc.

        [NotMapped]
        [Display(Name = "Full Name")]
        public string FullName => string.Join(" ", new[] { Title, FirstName, LastName, Suffix }
                                         .Where(s => !string.IsNullOrWhiteSpace(s)));

        [Required]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Invalid email address format")]
        [StringLength(100, ErrorMessage = "Email address cannot exceed 100 characters")]
        [Display(Name = "Email Address")]
        public string EmailAddress { get; set; }

        [Required]
        [DataType(DataType.PhoneNumber)]
        [RegularExpression(@"^[0-9\-\+\(\)\s]*$", ErrorMessage = "Invalid phone number format")]
        [StringLength(20, ErrorMessage = "Phone number cannot exceed 20 characters")]
        [Display(Name = "Phone Number")]
        public string PhoneNumber { get; set; }

        [Display(Name = "Position")]
        public int? PositionID { get; set; }
        public PlantillaPosition? PlantillaPosition { get; set; } // Navigation Property

        [Display(Name = "Office")]
        public int? OfficeID { get; set; }
        public Office? Office { get; set; }  // ✅ nullable, an employee can have an office but not always

        [DefaultValue(false)]
        [Display(Name = "Accountable Person")]
        public bool IsAccountablePerson { get; set; }// ✅ Ensures a default value in the database

        [DefaultValue(true)]
        [Display(Name = "Active")]
        public bool IsActive { get; set; } = true;

        public ICollection<UserAccount> UserAccounts { get; set; } = [];
    }
}
