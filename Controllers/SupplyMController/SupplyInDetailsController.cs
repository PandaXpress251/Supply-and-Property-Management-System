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
    public class SupplyInDetailsController : Controller
    {
        private readonly SIETEContext _context;

        public SupplyInDetailsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: SupplyInDetails
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.SupplyInDetail.Include(s => s.Supply).Include(s => s.SupplyTransaction);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: SupplyInDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyInDetail = await _context.SupplyInDetail
                .Include(s => s.Supply)
                .Include(s => s.SupplyTransaction)
                .FirstOrDefaultAsync(m => m.DetailID == id);
            if (supplyInDetail == null)
            {
                return NotFound();
            }

            return View(supplyInDetail);
        }

        // GET: SupplyInDetails/Create
        public IActionResult Create()
        {
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo");
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID");
            return View();
        }

        // POST: SupplyInDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetailID,TransactionID,SupplyID,Quantity,UnitCost,TotalAmount")] SupplyInDetail supplyInDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplyInDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyInDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyInDetail.TransactionID);
            return View(supplyInDetail);
        }

        // GET: SupplyInDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyInDetail = await _context.SupplyInDetail.FindAsync(id);
            if (supplyInDetail == null)
            {
                return NotFound();
            }
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyInDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyInDetail.TransactionID);
            return View(supplyInDetail);
        }

        // POST: SupplyInDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetailID,TransactionID,SupplyID,Quantity,UnitCost,TotalAmount")] SupplyInDetail supplyInDetail)
        {
            if (id != supplyInDetail.DetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplyInDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyInDetailExists(supplyInDetail.DetailID))
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
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyInDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyInDetail.TransactionID);
            return View(supplyInDetail);
        }

        // GET: SupplyInDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyInDetail = await _context.SupplyInDetail
                .Include(s => s.Supply)
                .Include(s => s.SupplyTransaction)
                .FirstOrDefaultAsync(m => m.DetailID == id);
            if (supplyInDetail == null)
            {
                return NotFound();
            }

            return View(supplyInDetail);
        }

        // POST: SupplyInDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplyInDetail = await _context.SupplyInDetail.FindAsync(id);
            if (supplyInDetail != null)
            {
                _context.SupplyInDetail.Remove(supplyInDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyInDetailExists(int id)
        {
            return _context.SupplyInDetail.Any(e => e.DetailID == id);
        }
    }
}
