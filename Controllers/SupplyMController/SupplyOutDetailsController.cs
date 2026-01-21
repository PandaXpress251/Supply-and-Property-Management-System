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
    public class SupplyOutDetailsController : Controller
    {
        private readonly SIETEContext _context;

        public SupplyOutDetailsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: SupplyOutDetails
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.SupplyOutDetail.Include(s => s.Office).Include(s => s.Supply).Include(s => s.SupplyTransaction);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: SupplyOutDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyOutDetail = await _context.SupplyOutDetail
                .Include(s => s.Office)
                .Include(s => s.Supply)
                .Include(s => s.SupplyTransaction)
                .FirstOrDefaultAsync(m => m.DetailID == id);
            if (supplyOutDetail == null)
            {
                return NotFound();
            }

            return View(supplyOutDetail);
        }

        // GET: SupplyOutDetails/Create
        public IActionResult Create()
        {
            ViewData["OfficeID"] = new SelectList(_context.Office, "OfficeID", "Acronym");
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo");
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID");
            return View();
        }

        // POST: SupplyOutDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("DetailID,TransactionID,SupplyID,Quantity,UnitCost,OfficeID")] SupplyOutDetail supplyOutDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(supplyOutDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["OfficeID"] = new SelectList(_context.Office, "OfficeID", "Acronym", supplyOutDetail.OfficeID);
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyOutDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyOutDetail.TransactionID);
            return View(supplyOutDetail);
        }

        // GET: SupplyOutDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyOutDetail = await _context.SupplyOutDetail.FindAsync(id);
            if (supplyOutDetail == null)
            {
                return NotFound();
            }
            ViewData["OfficeID"] = new SelectList(_context.Office, "OfficeID", "Acronym", supplyOutDetail.OfficeID);
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyOutDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyOutDetail.TransactionID);
            return View(supplyOutDetail);
        }

        // POST: SupplyOutDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("DetailID,TransactionID,SupplyID,Quantity,UnitCost,OfficeID")] SupplyOutDetail supplyOutDetail)
        {
            if (id != supplyOutDetail.DetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(supplyOutDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!SupplyOutDetailExists(supplyOutDetail.DetailID))
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
            ViewData["OfficeID"] = new SelectList(_context.Office, "OfficeID", "Acronym", supplyOutDetail.OfficeID);
            ViewData["SupplyID"] = new SelectList(_context.Supply, "SupplyID", "StockPropNo", supplyOutDetail.SupplyID);
            ViewData["TransactionID"] = new SelectList(_context.SupplyTransaction, "TransactionID", "TransactionID", supplyOutDetail.TransactionID);
            return View(supplyOutDetail);
        }

        // GET: SupplyOutDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var supplyOutDetail = await _context.SupplyOutDetail
                .Include(s => s.Office)
                .Include(s => s.Supply)
                .Include(s => s.SupplyTransaction)
                .FirstOrDefaultAsync(m => m.DetailID == id);
            if (supplyOutDetail == null)
            {
                return NotFound();
            }

            return View(supplyOutDetail);
        }

        // POST: SupplyOutDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var supplyOutDetail = await _context.SupplyOutDetail.FindAsync(id);
            if (supplyOutDetail != null)
            {
                _context.SupplyOutDetail.Remove(supplyOutDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool SupplyOutDetailExists(int id)
        {
            return _context.SupplyOutDetail.Any(e => e.DetailID == id);
        }
    }
}
