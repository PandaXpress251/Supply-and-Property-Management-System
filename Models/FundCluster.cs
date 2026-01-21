using System.ComponentModel.DataAnnotations;

namespace SIETE.Models
{
    public class FundCluster
    {
        [Required]
        public int FundClusterID { get; set; }

        [Required]
        public string FundClusterCode { get; set; }

        [Required]
        public string FundClusterName { get; set; }
    }
}