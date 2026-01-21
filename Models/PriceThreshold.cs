using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace SIETE.Models
{
    public class PriceThreshold
    {
        [Key]
        public int Id { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal PropertyThreshold { get; set; }

        [Required, Column(TypeName = "decimal(18,2)")]
        public decimal SPThreshold { get; set; }

        public DateTime EffectiveDate { get; set; } = DateTime.Now;

        internal static object OrderByDescending(Func<object, object> value)
        {
            throw new NotImplementedException();
        }
    }

}
