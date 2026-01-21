using System;
using System.Threading.Tasks;
using SIETE.Data;
using SIETE.Models.Property;
using Microsoft.EntityFrameworkCore;
using SIETE.Models;

namespace SIETE.Helpers
{
    public static class PropertyHelper
    {
        /// <summary>
        /// Applies the appropriate category to a property based on the price threshold.
        /// </summary>
        public static void ApplyCategoryLogic(Property property, PriceThreshold threshold)
        {

            // Logic to determine the category based on unit cost and the thresholds
            if (property.UnitCost >= threshold.PropertyThreshold)
            {
                property.Category = "Property";  // Low-value property
            }
            else if (property.UnitCost < threshold.SPThreshold)
            {
                property.Category = "Low-Valued SP";  // Low-valued special property
            }
            else
            {
                property.Category = "High-Valued SP";  // High-value special property
            }
        }

      
    }
}
