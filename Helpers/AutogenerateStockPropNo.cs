using SIETE.Data;
using SIETE.Models.Property;
using Microsoft.EntityFrameworkCore;

namespace SIETE.Helpers
{
    public class AutogenerateStockPropNo
    {
        /// <summary>
        /// Generates a Stock Property Number (StockPropNo) based on the unit cost and current date.
        /// The format is: [Category]-[yyyyMMdd]-[SequentialNumber]
        /// </summary>
        public static async Task<string> GenerateStockPropNoAsync(decimal unitCost, SIETEContext context)
        {
            if (context == null)
                throw new ArgumentNullException(nameof(context));

            string prefix = unitCost >= 15000 ? "PPE" : "SEP";
            string formattedDate = DateTime.Now.ToString("yyyyMMdd");

            // Find the last StockPropNo with the same prefix and date
            var lastProp = await context.Property
                .Where(p => p.StockPropNo.StartsWith($"{prefix}-{formattedDate}"))
                .OrderByDescending(p => p.StockPropNo)
                .FirstOrDefaultAsync();

            int nextNumber = 1;

            if (lastProp != null)
            {
                // Extract the last 5 digits and increment
                string[] parts = lastProp.StockPropNo.Split('-');
                if (parts.Length == 3 && int.TryParse(parts[2], out int lastNumber))
                {
                    nextNumber = lastNumber + 1;
                }
            }

            string formattedNumber = nextNumber.ToString("D5");
            return $"{prefix}-{formattedDate}-{formattedNumber}";
        }

    }
}
