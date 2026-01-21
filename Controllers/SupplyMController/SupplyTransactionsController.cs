using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models.Supply;

namespace SIETE.Controllers.SupplyMController
{
    public class SupplyTransactionsController : Controller
    {
        private readonly SIETEContext _context;

        public SupplyTransactionsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: SupplyTransactions
        public async Task<IActionResult> Index()
        {
            return View(await _context.SupplyTransaction.ToListAsync());
        }

        // GET: SupplyTransactions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyTransaction = await _context.SupplyTransaction
                .Include(t => t.SupplyInDetails)
                    .ThenInclude(d => d.Supply)
                .Include(t => t.SupplyOutDetails)
                    .ThenInclude(d => d.Supply)
                .Include(t => t.SupplyOutDetails)
                 .ThenInclude(d => d.Office)
                .FirstOrDefaultAsync(m => m.TransactionID == id);

            if (supplyTransaction == null)
            {
                return NotFound();
            }

            return PartialView("Details", supplyTransaction);
        }

        public IActionResult SupplyIN()
        {
            var viewModel = new SupplyTransactionVM
            {
                Transaction = new SupplyTransaction { Type = TransactionType.IN },
                SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName"),
                FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplyIN(SupplyTransactionVM supplyVM)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Reload the lists if validation fails
            //    supplyVM.SupplierList = new SelectList(_context.Supplier, "SupplierID", "SupplierName");
            //    supplyVM.FundClusterList = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterName");
            //    return View(supplyVM);
            //}

            // Calculate total amount
            decimal totalAmount = supplyVM.SupplyInDetails.Sum(d => d.Supply.StockQuantity * d.Supply.UnitCost);

            // Create transaction
            var transaction = new SupplyTransaction
            {
                Type = TransactionType.IN,
                Date = DateTime.Now,
                Remarks = supplyVM.Transaction.Remarks,
                TotalAmount = totalAmount
            };

            // Wrap everything in a transaction
            using var dbTransaction = await _context.Database.BeginTransactionAsync();
            try
            {
                // Save the main transaction first
                _context.SupplyTransaction.Add(transaction);
                await _context.SaveChangesAsync(); // Get TransactionID for detail records

                foreach (var detail in supplyVM.SupplyInDetails)
                {
                    var supply = new Supply
                    {
                        SupplierID = detail.Supply.SupplierID,
                        FundClusterID = detail.Supply.FundClusterID,
                        StockPropNo = detail.Supply.StockPropNo,
                        SupplyName = detail.Supply.SupplyName,
                        Description = detail.Supply.Description,
                        UnitMeasurement = detail.Supply.UnitMeasurement,
                        UnitCost = detail.Supply.UnitCost,
                        StockQuantity = detail.Supply.StockQuantity,
                        DateAcquired = DateTime.Now,
                        Status = Supply.StockCardStatus.Available
                    };

                    _context.Supply.Add(supply);
                    await _context.SaveChangesAsync(); // Get SupplyID

                    // Check for existing StockCard (normalize casing/spacing)
                    var stockCard = await _context.StockCard.FirstOrDefaultAsync(sc =>
                        sc.StockCardName.ToUpper().Trim() == supply.SupplyName.ToUpper().Trim() &&
                        sc.StockDescription.ToUpper().Trim() == (supply.Description ?? "").ToUpper().Trim() &&
                        sc.StockUnitMeasurement.ToUpper().Trim() == supply.UnitMeasurement.ToUpper().Trim());

                    if (stockCard == null)
                    {
                        stockCard = new StockCard
                        {
                            StockCardName = supply.SupplyName,
                            StockDescription = supply.Description ?? "",
                            StockUnitMeasurement = supply.UnitMeasurement,
                            CurrentStockQuantity = supply.StockQuantity,
                            TotalAmount = supply.StockQuantity * supply.UnitCost,
                            LastUpdated = DateTime.Now
                        };

                        _context.StockCard.Add(stockCard);
                        await _context.SaveChangesAsync();

                        // Update supply with StockCardID
                        supply.StockCardID = stockCard.StockCardID;
                        _context.Update(supply);
                    }
                    else
                    {
                        stockCard.CurrentStockQuantity += supply.StockQuantity;
                        stockCard.TotalAmount += supply.StockQuantity * supply.UnitCost;
                        stockCard.LastUpdated = DateTime.Now;
                        _context.StockCard.Update(stockCard);

                        // Update supply with existing StockCardID
                        supply.StockCardID = stockCard.StockCardID;
                        _context.Update(supply);
                    }

                    var supplyInDetail = new SupplyInDetail
                    {
                        TransactionID = transaction.TransactionID,
                        SupplyID = supply.SupplyID,
                        Quantity = supply.StockQuantity,
                        UnitCost = supply.UnitCost,
                        TotalAmount = supply.StockQuantity * supply.UnitCost
                    };

                    _context.SupplyInDetail.Add(supplyInDetail);
                }

                await _context.SaveChangesAsync();
                await dbTransaction.CommitAsync();

                // Show success message
                TempData["SuccessMessage"] = "Supply received successfully!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                await dbTransaction.RollbackAsync();
                ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                return View(supplyVM);
            }
        }



        public IActionResult SupplyOUT()
        {
            var viewModel = new SupplyTransactionVM
            {
                Transaction = new SupplyTransaction { Type = TransactionType.OUT },
                OfficeList = new SelectList(_context.Office.ToList(), "OfficeID", "OfficeName"),
                SupplyList = new SelectList(_context.Supply.Where(s => s.StockQuantity > 0), "SupplyID", "SupplyName")
            };
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> SupplyOUT(SupplyTransactionVM supplyVM)
        {
            //if (!ModelState.IsValid)
            //{
            //    // Reload the lists if validation fails
            //    supplyVM.OfficeList = new SelectList(_context.Office.ToList(), "OfficeID", "OfficeName");
            //    supplyVM.SupplyList = new SelectList(_context.Supply.Where(s => s.StockQuantity > 0), "SupplyID", "SupplyName");
            //    return View(supplyVM);
            //}

            // Validate stock availability
            foreach (var detail in supplyVM.SupplyOutDetails)
            {
                var supply = await _context.Supply.FindAsync(detail.SupplyID);
                if (supply == null)
                {
                    ModelState.AddModelError("", $"Supply not found");
                    return View(supplyVM);
                }
                if (supply.StockQuantity < detail.Quantity)
                {
                    ModelState.AddModelError("", $"Insufficient stock for {supply.SupplyName}");
                    return View(supplyVM);
                }
            }

            // Calculate total amount
            decimal totalAmount = supplyVM.SupplyOutDetails.Sum(d => d.Quantity * d.UnitCost);

            // Create transaction
            var transaction = new SupplyTransaction
            {
                Type = TransactionType.OUT,
                Date = DateTime.Now,
                Remarks = supplyVM.Transaction.Remarks,
                TotalAmount = totalAmount
            };

            // Wrap everything in a transaction
            using (var dbTransaction = await _context.Database.BeginTransactionAsync())
            {
                try
                {
                    // Save the main transaction first
                    _context.SupplyTransaction.Add(transaction);
                    await _context.SaveChangesAsync(); // Get TransactionID for detail records

                    foreach (var detail in supplyVM.SupplyOutDetails)
                    {
                        // Get the selected supply
                        var supply = await _context.Supply.FindAsync(detail.SupplyID);
                        if (supply == null)
                            continue;

                        // Deduct quantity from supply
                        supply.StockQuantity -= detail.Quantity;
                        supply.UpdateStockStatus(); // Update the status based on new quantity

                        _context.Supply.Update(supply);

                        // Find matching stock card
                        var stockCard = await _context.StockCard.FirstOrDefaultAsync(sc =>
                            sc.StockCardName == supply.SupplyName &&
                            sc.StockDescription == supply.Description &&
                            sc.StockUnitMeasurement == supply.UnitMeasurement);

                        if (stockCard != null)
                        {
                            stockCard.CurrentStockQuantity -= detail.Quantity;
                            stockCard.TotalAmount = stockCard.CurrentStockQuantity * supply.UnitCost;
                            stockCard.LastUpdated = DateTime.Now;

                            _context.StockCard.Update(stockCard);
                        }

                        var supplyOutDetail = new SupplyOutDetail
                        {
                            TransactionID = transaction.TransactionID,
                            SupplyID = supply.SupplyID,
                            Quantity = detail.Quantity,
                            UnitCost = supply.UnitCost,
                            OfficeID = detail.OfficeID
                        };

                        _context.SupplyOutDetail.Add(supplyOutDetail);
                    }

                    await _context.SaveChangesAsync();
                    await dbTransaction.CommitAsync();

                    // Show success message
                    TempData["SuccessMessage"] = "Supply issued successfully!";
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    await dbTransaction.RollbackAsync();
                    ModelState.AddModelError("", $"An error occurred: {ex.Message}");
                    return View(supplyVM);
                }
            }
        }

        // GET: SupplyTransactions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyTransaction = await _context.SupplyTransaction.FindAsync(id);
            if (supplyTransaction == null)
            {
                return NotFound();
            }
            return View(supplyTransaction);
        }

        // POST: SupplyTransactions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("TransactionID,Type,Date,Remarks,TotalAmount")] SupplyTransaction supplyTransaction)
        {
            if (id != supplyTransaction.TransactionID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplyTransaction);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyTransactionExists(supplyTransaction.TransactionID))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(supplyTransaction);
        }

        // GET: SupplyTransactions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyTransaction = await _context.SupplyTransaction
                .FirstOrDefaultAsync(m => m.TransactionID == id);
            if (supplyTransaction == null)
            {
                return NotFound();
            }

            return View(supplyTransaction);
        }

        // POST: SupplyTransactions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplyTransaction = await _context.SupplyTransaction.FindAsync(id);
            if (supplyTransaction != null)
            {
                _context.SupplyTransaction.Remove(supplyTransaction);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyTransactionExists(int id)
        {
            return _context.SupplyTransaction.Any(e => e.TransactionID == id);
        }

        [HttpGet]
        public async Task<IActionResult> GetSupplyDetails(int id)
        {
            var supply = await _context.Supply.FindAsync(id);
            if (supply == null)
            {
                return NotFound();
            }

            return Json(new
            {
                description = supply.Description,
                unit = supply.UnitMeasurement,
                stockQuantity = supply.StockQuantity,
                unitCost = supply.UnitCost
            });
        }
    }
}
