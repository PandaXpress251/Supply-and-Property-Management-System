using SIETE.Models.Property;
using SIETE.Models.Supply;

namespace SIETE.Models
{
    public class MovementIssueReportVM
    {
        public DateTime StartDate { get; set; } = DateTime.Now;

        // Use a single list to hold both supply and property transaction details
        public List<MovementIssueReportRowVM> ReportRows { get; set; } = new();
    }
}
