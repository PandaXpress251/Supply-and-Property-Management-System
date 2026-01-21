using SIETE.Models.Supply;
using System.Text.Json.Serialization;

namespace SIETE.Models
{
    public class ExtractedDataVM
    {
        [JsonPropertyName("supplierID")]
        public int SupplierID { get; set; }

        [JsonPropertyName("fundClusterID")]
        public int FundClusterID { get; set; }

        [JsonPropertyName("stockCardID")]
        public int? StockCardID { get; set; }

        [JsonPropertyName("propertyCardID")]
        public int? PropertyCardID { get; set; }

        [JsonPropertyName("stockPropNo")]
        public string StockPropNo { get; set; }

        [JsonPropertyName("itemName")]
        public string ItemName { get; set; }

        [JsonPropertyName("itemDescription")]
        public string ItemDescription { get; set; }

        [JsonPropertyName("itemUnitMeasurement")]
        public string ItemUnitMeasurement { get; set; }

        [JsonPropertyName("itemUnitCost")]
        public decimal ItemUnitCost { get; set; }

        [JsonPropertyName("itemStockQuantity")]
        public int ItemStockQuantity { get; set; }

        [JsonPropertyName("itemStatus")]
        public string ItemStatus { get; set; } = "Active";

        [JsonPropertyName("dateAcquired")]
        public DateTime DateAcquired { get; set; } = DateTime.UtcNow;

        [JsonPropertyName("itemCategory")]
        public string ItemCategory { get; set; }

        public SupplyTransaction Transaction { get; set; }
    }
}

