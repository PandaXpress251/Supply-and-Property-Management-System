namespace SIETE.Models
{
    public class MovementIssueReportRowVM
    {
        public string ResponsibilityCode { get; set; }
        public string StockPropNo { get; set; }
        public string ItemName { get; set; }
        public string Unit { get; set; }
        public int Quantity { get; set; }
        public decimal UnitCost { get; set; }
        public decimal TotalAmount { get; set; }
    }
}
