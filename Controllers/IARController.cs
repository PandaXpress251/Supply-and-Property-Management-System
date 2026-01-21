using SIETE.Data;
using SIETE.Helpers;
using SIETE.Models;
using SIETE.Models.Property;
using SIETE.Models.Supply;
using ExcelDataReader;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Data;
using System.Text;

namespace SIETE.Controllers
{
    public class IARController : Controller
    {
        private readonly SIETEContext _context;

        public IARController(SIETEContext    context)
        {
            _context = context;
        }

        private async Task<PriceThreshold?> GetLatestThreshold()
        {
            return await _context.PriceThreshold
                .OrderByDescending(t => t.EffectiveDate)
                .FirstOrDefaultAsync();
        }

        [HttpGet]
        public IActionResult IARUpload()
        {
            var viewModel = new SupplyTransactionVM
            {
                Supply = new Supply(),
                SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName"),
                FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode")
            };

            return View(viewModel);
        }

        [HttpPost]
        public async Task<IActionResult> ExtractExcelData(IFormFile file)
        {
            if (file == null || file.Length == 0)
            {
                return BadRequest(new { success = false, message = "No file uploaded." });
            }

            try
            {
                // Ensure encoding provider is registered for proper handling of file encodings
                System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
                var suppliesList = new List<ExtractedDataVM>();

                using (var stream = new MemoryStream())
                {
                    await file.CopyToAsync(stream);
                    stream.Position = 0;

                    using (var reader = ExcelReaderFactory.CreateReader(stream))
                    {
                        var dataSet = reader.AsDataSet();
                        var table = dataSet.Tables[0];

                        // Process each row starting from the 16th row (index 15)
                        for (int i = 16; i < table.Rows.Count; i++)
                        {
                            var row = table.Rows[i];

                            // Stop processing if Column C contains "- Nothing Follows -"
                            if (row[2]?.ToString().Trim() == "- Nothing Follows -")
                            {
                                break;
                            }

                            // Extract and split SupplyName and Description from Column C
                            string[] itemParts = row[2]?.ToString().Split(',', 2, StringSplitOptions.RemoveEmptyEntries);
                            string supplyName = itemParts.Length > 0 ? itemParts[0].Trim() : "";
                            string description = itemParts.Length > 1 ? itemParts[1].Trim() : "";

                            // Validate required fields: StockPropNo, UnitMeasurement, SupplyName, StockQuantity, and UnitCost
                            if (string.IsNullOrWhiteSpace(row[0]?.ToString()) ||  // StockPropNo
                                string.IsNullOrWhiteSpace(row[1]?.ToString()) ||  // UnitMeasurement
                                string.IsNullOrWhiteSpace(supplyName) ||         // SupplyName
                                !int.TryParse(row[3]?.ToString(), out int qty) || qty <= 0 || // StockQuantity
                                !decimal.TryParse(row[4]?.ToString(), out decimal price) || price <= 0) // UnitCost
                            {
                                continue; // Skip this row if any required field is missing or invalid
                            }

                            // Create an ExtractedDataVM object and add it to the list
                            var extractedData = new ExtractedDataVM
                            {

                                StockPropNo = row[0]?.ToString().Trim(),
                                ItemUnitMeasurement = row[1]?.ToString().Trim(),
                                ItemName = supplyName,
                                ItemDescription = description,
                                ItemStockQuantity = qty,
                                ItemUnitCost = price,
                                ItemCategory = "Supply", // Assuming "Supply", can adjust later for "Property"
                                DateAcquired = DateTime.UtcNow
                            };

                            suppliesList.Add(extractedData);
                        }
                    }
                }

                return Json(new { success = true, supplies = suppliesList });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message, stackTrace = ex.StackTrace });
            }
        }


        //Saving Extracted file
        [HttpPost]
        public async Task<IActionResult> SaveSupplyTransaction([FromBody] List<ExtractedDataVM> extracted)
        {
            if (extracted == null || extracted.Count == 0)
                return Json(new { success = false, message = "No extracted data provided." });

            using var dbTransaction = await _context.Database.BeginTransactionAsync();

            try
            {
                int supplierID = extracted[0].SupplierID;
                int fundClusterID = extracted[0].FundClusterID;
                var threshold = await GetLatestThreshold();

                var supplies = extracted.Where(e => e.ItemCategory == "Supply").ToList();
                var properties = extracted.Where(e => e.ItemCategory == "Property").ToList();

                SupplyTransaction supplyTransaction = null;
                PropertyTransaction propertyTransaction = null;

                if (supplies.Any())
                {
                    supplyTransaction = new SupplyTransaction
                    {
                        Type = TransactionType.IN,
                        Date = DateTime.UtcNow,
                        Remarks = "Item In via IAR scanning",
                    };
                    _context.SupplyTransaction.Add(supplyTransaction);
                    await _context.SaveChangesAsync();
                }

                if (properties.Any())
                {
                    propertyTransaction = new PropertyTransaction
                    {
                        Type = TransactionTypes.Aquired,
                        TransactionDate = DateTime.UtcNow,
                        Remarks = "Item In via IAR scanning",
                    };
                    _context.PropertyTransaction.Add(propertyTransaction);
                    await _context.SaveChangesAsync();
                }

                // === SUPPLY LOGIC ===
                foreach (var item in supplies)
                {
                    // Check for existing StockCard with the same properties
                    var existingStockCard = await _context.StockCard
                        .FirstOrDefaultAsync(sc => 
                            sc.StockCardName == item.ItemName && 
                            sc.StockDescription == item.ItemDescription && 
                            sc.StockUnitMeasurement == item.ItemUnitMeasurement);

                    var supply = new Supply
                    {
                        StockPropNo = item.StockPropNo,
                        SupplyName = item.ItemName,
                        Description = item.ItemDescription,
                        UnitMeasurement = item.ItemUnitMeasurement,
                        UnitCost = item.ItemUnitCost,
                        StockQuantity = item.ItemStockQuantity,
                        Status = Supply.StockCardStatus.Available,
                        DateAcquired = item.DateAcquired,
                        SupplierID = supplierID,
                        FundClusterID = fundClusterID
                    };

                    _context.Supply.Add(supply);
                    await _context.SaveChangesAsync();

                    if (existingStockCard != null)
                    {
                        // Update existing StockCard
                        existingStockCard.CurrentStockQuantity += supply.StockQuantity;
                        existingStockCard.TotalAmount += supply.StockQuantity * supply.UnitCost;
                        existingStockCard.LastUpdated = DateTime.UtcNow;
                        _context.StockCard.Update(existingStockCard);
                        supply.StockCardID = existingStockCard.StockCardID;
                    }
                    else
                    {
                        // Create new StockCard if none exists
                        var newStockCard = new StockCard
                        {
                            StockCardName = supply.SupplyName,
                            StockDescription = supply.Description ?? "",
                            StockUnitMeasurement = supply.UnitMeasurement,
                            CurrentStockQuantity = supply.StockQuantity,
                            TotalAmount = supply.StockQuantity * supply.UnitCost,
                            LastUpdated = DateTime.UtcNow
                        };
                        _context.StockCard.Add(newStockCard);
                        await _context.SaveChangesAsync();
                        supply.StockCardID = newStockCard.StockCardID;
                    }

                    _context.Supply.Update(supply);

                    _context.SupplyInDetail.Add(new SupplyInDetail
                    {
                        TransactionID = supplyTransaction.TransactionID,
                        SupplyID = supply.SupplyID,
                        Quantity = supply.StockQuantity,
                        UnitCost = supply.UnitCost
                    });
                }

                // === PROPERTY LOGIC ===
                foreach (var item in properties)
                {
                    var property = new Property
                    {
                        StockPropNo = item.StockPropNo,
                        SupplierID = item.SupplierID,
                        FundClusterID = item.FundClusterID,
                        PropertyName = item.ItemName,
                        Description = item.ItemDescription,
                        UnitMeasurement = item.ItemUnitMeasurement,
                        UnitCost = item.ItemUnitCost,
                        StockQuantity = item.ItemStockQuantity,
                        DateAcquired = DateTime.Now,
                        Status = "Active"
                    };

                    if (threshold != null)
                        PropertyHelper.ApplyCategoryLogic(property, threshold);

                    _context.Property.Add(property);
                    await _context.SaveChangesAsync();

                    PropertyCard propertyCard;
                    if (item.PropertyCardID.HasValue)
                    {
                        propertyCard = await _context.PropertyCard.FindAsync(item.PropertyCardID.Value);
                        if (propertyCard != null)
                        {
                            propertyCard.CurrentStockQuantity += property.StockQuantity;
                            propertyCard.TotalAmount += property.StockQuantity * property.UnitCost;
                            propertyCard.LastUpdated = DateTime.UtcNow;
                            _context.PropertyCard.Update(propertyCard);
                        }
                    }
                    else
                    {
                        propertyCard = new PropertyCard
                        {
                            PropertyCardName = property.PropertyName,
                            PropertyDescription = property.Description ?? "",
                            PropertyUnitMeasurement = property.UnitMeasurement,
                            CurrentStockQuantity = property.StockQuantity,
                            TotalAmount = property.StockQuantity * property.UnitCost,
                            LastUpdated = DateTime.UtcNow
                        };
                        _context.PropertyCard.Add(propertyCard);
                        await _context.SaveChangesAsync();
                    }

                    property.PropertyCardID = propertyCard.PropertyCardID;
                    _context.Property.Update(property);

                    _context.PropertyTransactionDetail.Add(new PropertyTransactionDetail
                    {
                        TransactionID = propertyTransaction.TransactionID,
                        PropertyID = property.PropertyID,
                        Quantity = property.StockQuantity,
                        UnitCost = property.UnitCost
                    });
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                return Json(new
                {
                    success = true,
                    message = "✔ Items successfully saved via IAR scan.",
                    Url = Url.Action("Index", "SupplyTransactions")
                });
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                return Json(new
                {
                    success = false,
                    message = "❌ Failed to save items. Error: " + ex.Message
                });
            }
        }


    }
}
