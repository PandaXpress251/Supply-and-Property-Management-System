using Microsoft.AspNetCore.Mvc.Rendering;

namespace SIETE.Models.Property
{
    public class PropertyTransactionVM
    {
        public PropertyTransaction Transaction { get; set; }
        public SelectList SupplierList { get; set; }
        public SelectList FundClusterList { get; set; }
        public SelectList OfficeList { get; set; }
        public SelectList PropertyList { get; set; }

        public SelectList TransferToEmployeeList { get; set; }
        public List<PropertyTransactionDetail> PropertyTransactionDetail { get; set; } = new();
        public SelectList EmployeeList { get; set; }
        public int EmployeeID { get; set; }
        public int TransferToEmployeeID { get; set; }
        public string DisposalSource { get; set; } = "stock"; // Default value is "stock"
    }
}
