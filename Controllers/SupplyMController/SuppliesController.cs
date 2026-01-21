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
    public class SuppliesController : Controller
    {
        private readonly SIETEContext _context;

        public SuppliesController(SIETEContext context)
        {
            _context = context;
        }

        // GET: Supplies
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.Supply
                .Include(s => s.FundCluster)
                .Include(s => s.StockCard)
                .Include(s => s.Supplier)
                .OrderByDescending(s => s.DateAcquired);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: Supplies/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supply = await _context.Supply
                .Include(s => s.FundCluster)
                .Include(s => s.StockCard)
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.SupplyID == id);
            if (supply == null)
            {
                return NotFound();
            }

            return View(supply);
        }

        // GET: Supplies/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supply = await _context.Supply
                .Include(s => s.StockCard)
                .FirstOrDefaultAsync(s => s.SupplyID == id);

            if (supply == null)
            {
                return NotFound();
            }

            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode", supply.FundClusterID);
            ViewData["StockCardID"] = new SelectList(_context.Set<StockCard>(), "StockCardID", "StockCardName", supply.StockCardID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierName", supply.SupplierID);
            return View(supply);
        }

        // POST: Supplies/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SupplyID,SupplierID,FundClusterID,StockCardID,StockPropNo,SupplyName,Description,UnitMeasurement,UnitCost,StockQuantity,DateAcquired,Status")] Supply supply, string newStockCardName = null)
        {
            if (id != supply.SupplyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var originalSupply = await _context.Supply
                        .Include(s => s.StockCard)
                        .FirstOrDefaultAsync(s => s.SupplyID == id);

                    if (originalSupply == null)
                    {
                        return NotFound();
                    }

                    // Handle new stock card creation if provided
                    if (!string.IsNullOrWhiteSpace(newStockCardName))
                    {
                        var newStockCard = new StockCard
                        {
                            StockCardName = newStockCardName,
                            CurrentStockQuantity = supply.StockQuantity,
                            TotalAmount = supply.StockQuantity * supply.UnitCost,
                            LastUpdated = DateTime.Now
                        };

                        _context.StockCard.Add(newStockCard);
                        await _context.SaveChangesAsync(); // Save to get the new StockCardID

                        // Update the supply with the new stock card
                        supply.StockCardID = newStockCard.StockCardID;

                        // Update old stock card if exists
                        if (originalSupply.StockCardID.HasValue)
                        {
                            var oldStockCard = await _context.StockCard.FindAsync(originalSupply.StockCardID);
                            if (oldStockCard != null)
                            {
                                oldStockCard.CurrentStockQuantity -= originalSupply.StockQuantity;
                                oldStockCard.TotalAmount = oldStockCard.CurrentStockQuantity * originalSupply.UnitCost;
                                _context.StockCard.Update(oldStockCard);
                            }
                        }
                    }
                    else
                    {
                        // Check if quantity is being changed
                        if (originalSupply.StockQuantity != supply.StockQuantity)
                        {
                            TempData["WarningMessage"] = "Warning: Changing the quantity will affect stock levels. Please ensure this is correct.";
                        }

                        // Update stock card if needed
                        if (originalSupply.StockCardID != supply.StockCardID)
                        {
                            var oldStockCard = await _context.StockCard.FindAsync(originalSupply.StockCardID);
                            if (oldStockCard != null)
                            {
                                oldStockCard.CurrentStockQuantity -= originalSupply.StockQuantity;
                                oldStockCard.TotalAmount = oldStockCard.CurrentStockQuantity * originalSupply.UnitCost;
                                _context.StockCard.Update(oldStockCard);
                            }

                            var newStockCard = await _context.StockCard.FindAsync(supply.StockCardID);
                            if (newStockCard != null)
                            {
                                newStockCard.CurrentStockQuantity += supply.StockQuantity;
                                newStockCard.TotalAmount = newStockCard.CurrentStockQuantity * supply.UnitCost;
                                _context.StockCard.Update(newStockCard);
                            }
                        }
                        else
                        {
                            // Update existing stock card
                            var stockCard = await _context.StockCard.FindAsync(supply.StockCardID);
                            if (stockCard != null)
                            {
                                stockCard.CurrentStockQuantity = stockCard.CurrentStockQuantity - originalSupply.StockQuantity + supply.StockQuantity;
                                stockCard.TotalAmount = stockCard.CurrentStockQuantity * supply.UnitCost;
                                _context.StockCard.Update(stockCard);
                            }
                        }
                    }

                    // Log the changes
                    var changes = new List<string>();
                    if (originalSupply.SupplyName != supply.SupplyName)
                        changes.Add($"Name: {originalSupply.SupplyName} → {supply.SupplyName}");
                    if (originalSupply.Description != supply.Description)
                        changes.Add($"Description: {originalSupply.Description} → {supply.Description}");
                    if (originalSupply.UnitMeasurement != supply.UnitMeasurement)
                        changes.Add($"Unit: {originalSupply.UnitMeasurement} → {supply.UnitMeasurement}");
                    if (originalSupply.UnitCost != supply.UnitCost)
                        changes.Add($"Unit Cost: {originalSupply.UnitCost} → {supply.UnitCost}");
                    if (originalSupply.StockQuantity != supply.StockQuantity)
                        changes.Add($"Quantity: {originalSupply.StockQuantity} → {supply.StockQuantity}");
                    if (originalSupply.StockCardID != supply.StockCardID)
                    {
                        if (!string.IsNullOrWhiteSpace(newStockCardName))
                            changes.Add($"Stock Card: Created new stock card '{newStockCardName}'");
                        else
                            changes.Add($"Stock Card: {originalSupply.StockCardID} → {supply.StockCardID}");
                    }

                    if (changes.Any())
                    {
                        TempData["ChangeLog"] = string.Join(", ", changes);
                    }

                    _context.Update(supply);
                    await _context.SaveChangesAsync();
                    TempData["SuccessMessage"] = "Supply updated successfully.";
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyExists(supply.SupplyID))
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
            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode", supply.FundClusterID);
            ViewData["StockCardID"] = new SelectList(_context.Set<StockCard>(), "StockCardID", "StockCardName", supply.StockCardID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "SupplierName", supply.SupplierID);
            return View(supply);
        }

        // GET: Supplies/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supply = await _context.Supply
                .Include(s => s.FundCluster)
                .Include(s => s.StockCard)
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.SupplyID == id);

            if (supply == null)
            {
                return NotFound();
            }

            return View(supply);
        }

        // POST: Supplies/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supply = await _context.Supply
                .Include(s => s.StockCard)
                .FirstOrDefaultAsync(s => s.SupplyID == id);

            if (supply == null)
            {
                return NotFound();
            }

            // Update stock card
            if (supply.StockCardID.HasValue)
            {
                var stockCard = await _context.StockCard.FindAsync(supply.StockCardID);
                if (stockCard != null)
                {
                    stockCard.CurrentStockQuantity -= supply.StockQuantity;
                    stockCard.TotalAmount = stockCard.CurrentStockQuantity * supply.UnitCost;
                    _context.StockCard.Update(stockCard);
                }
            }

            _context.Supply.Remove(supply);
            await _context.SaveChangesAsync();
            TempData["SuccessMessage"] = "Supply deleted successfully.";
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyExists(int id)
        {
            return _context.Supply.Any(e => e.SupplyID == id);
        }
    }
}
