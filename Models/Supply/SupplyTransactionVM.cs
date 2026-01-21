using Microsoft.AspNetCore.Mvc.Rendering;
using System.Collections.Generic;

namespace SIETE.Models.Supply
{
    public class SupplyTransactionVM
    {
        public SupplyTransaction Transaction { get; set; }
        public Supply Supply { get; set; }
        public SelectList SupplierList { get; set; }
        public SelectList FundClusterList { get; set; }
        public SelectList OfficeList { get; set; }
        public SelectList SupplyList { get; set; }
        public List<SupplyInDetail> SupplyInDetails { get; set; } = new();
        public List<SupplyOutDetail> SupplyOutDetails { get; set; } = new();
    }
}
