using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace SIETE.Models
{
    public class Office
    {
        [Key] // Unique Office Id
        public int OfficeID { get; set; }

        [Required] // Office name
        public string OfficeName { get; set; }

        [Required] // Acronym
        public string Acronym { get; set; }

        [Required] // Office type
        public string OfficeType { get; set; }

        [Required] // Responsibility Center Code
        public string RespCenter_Code { get; set; }

        [Required] // Parent Code
        public string Parent_Code { get; set; }

        //If the office is still active?

        [Required]
        [DefaultValue(true)]
        public bool IsActive { get; set; } = true;

        public override string ToString() => $"{Acronym} - {OfficeName}";

        // Navigation Property (Collection of Employees)
        public virtual ICollection<Employee> Employees { get; set; } = [];
    }
}
