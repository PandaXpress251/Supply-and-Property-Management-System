using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models;
using SIETE.Models.Property;

namespace SIETE.Controllers
{
    public class ReportController : Controller
    {
        private readonly SIETEContext _context;

        public ReportController(SIETEContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> MovementIssueReport(DateTime? startDate)
        {
            if (startDate == null)
            {
                // Render the view with empty model for the form
                return View(new MovementIssueReportVM());
            }

            DateTime endDate = DateTime.Now;

            // Fetch data
            var supplyOutDetails = await _context.SupplyOutDetail
                .Where(s => s.SupplyTransaction.Date >= startDate && s.SupplyTransaction.Date <= endDate)
                .Include(s => s.Supply)
                .Include(s => s.Office)
                .AsNoTracking()
                .ToListAsync();

            var propertyTransactionDetails = await _context.PropertyTransactionDetail
     .Where(p => p.PropertyTransaction.TransactionDate >= startDate
             && p.PropertyTransaction.TransactionDate <= endDate
             && p.PropertyTransaction.Type == TransactionTypes.Transferred)
     .Include(p => p.Property)
     .Include(p => p.Employee)          // Include Employee
         .ThenInclude(e => e.Office)    // Include Office to get RespCenter_Code
     .AsNoTracking()
     .ToListAsync();

            var reportRows = new List<MovementIssueReportRowVM>();

            // Map Supply
            foreach (var supply in supplyOutDetails)
            {
                reportRows.Add(new MovementIssueReportRowVM
                {
                    ResponsibilityCode = supply.Office.RespCenter_Code ,
                    StockPropNo = supply.Supply?.StockPropNo ?? "",
                    ItemName = supply.Supply?.SupplyName +", "+supply.Supply.Description ?? "",
                    Unit = supply.Supply?.UnitMeasurement ?? "",
                    Quantity = supply.Quantity,
                    UnitCost = supply.UnitCost,
                    TotalAmount = supply.Quantity * supply.UnitCost
                });
            }

            // Map Property
            foreach (var property in propertyTransactionDetails)
            {
                reportRows.Add(new MovementIssueReportRowVM
                {
                    ResponsibilityCode = property.Employee?.Office?.RespCenter_Code ?? "",
                    StockPropNo = property.Property?.StockPropNo ?? "",
                    ItemName = property.Property.PropertyName + ", " + property.Property.Description,
                    Unit = property.Property?.UnitMeasurement ?? "",
                    Quantity = property.Quantity,
                    UnitCost = property.UnitCost,
                    TotalAmount = property.TotalAmount
                });
            }

            var viewModel = new MovementIssueReportVM
            {
                StartDate = startDate.Value,
                ReportRows = reportRows
            };

            return View(viewModel);
        }

    }
}
