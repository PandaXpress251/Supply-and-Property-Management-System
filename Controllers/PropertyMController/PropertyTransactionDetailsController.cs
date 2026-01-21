using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models.Property;

namespace SIETE.Controllers.PropertyMController
{
    public class PropertyTransactionDetailsController : Controller
    {
        private readonly SIETEContext _context;

        public PropertyTransactionDetailsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PropertyTransactionDetails
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.PropertyTransactionDetail.Include(p => p.Property).Include(p => p.PropertyTransaction);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: PropertyTransactionDetails/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTransactionDetail = await _context.PropertyTransactionDetail
                .Include(p => p.Property)
                .Include(p => p.PropertyTransaction)
                .FirstOrDefaultAsync(m => m.PropertyTransactionDetailID == id);
            if (propertyTransactionDetail == null)
            {
                return NotFound();
            }

            return View(propertyTransactionDetail);
        }

        // GET: PropertyTransactionDetails/Create
        public IActionResult Create()
        {
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category");
            ViewData["TransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID");
            return View();
        }

        // POST: PropertyTransactionDetails/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyTransactionDetailID,TransactionID,PropertyID,Quantity,UnitCost,TotalAmount,EmployeeID,Location,DisposalType,AssignmentID")] PropertyTransactionDetail propertyTransactionDetail)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyTransactionDetail);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyTransactionDetail.PropertyID);
            ViewData["TransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyTransactionDetail.TransactionID);
            return View(propertyTransactionDetail);
        }

        // GET: PropertyTransactionDetails/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTransactionDetail = await _context.PropertyTransactionDetail.FindAsync(id);
            if (propertyTransactionDetail == null)
            {
                return NotFound();
            }
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyTransactionDetail.PropertyID);
            ViewData["TransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyTransactionDetail.TransactionID);
            return View(propertyTransactionDetail);
        }

        // POST: PropertyTransactionDetails/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyTransactionDetailID,TransactionID,PropertyID,Quantity,UnitCost,TotalAmount,EmployeeID,Location,DisposalType,AssignmentID")] PropertyTransactionDetail propertyTransactionDetail)
        {
            if (id != propertyTransactionDetail.PropertyTransactionDetailID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyTransactionDetail);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyTransactionDetailExists(propertyTransactionDetail.PropertyTransactionDetailID))
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
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyTransactionDetail.PropertyID);
            ViewData["TransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyTransactionDetail.TransactionID);
            return View(propertyTransactionDetail);
        }

        // GET: PropertyTransactionDetails/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyTransactionDetail = await _context.PropertyTransactionDetail
                .Include(p => p.Property)
                .Include(p => p.PropertyTransaction)
                .FirstOrDefaultAsync(m => m.PropertyTransactionDetailID == id);
            if (propertyTransactionDetail == null)
            {
                return NotFound();
            }

            return View(propertyTransactionDetail);
        }

        // POST: PropertyTransactionDetails/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyTransactionDetail = await _context.PropertyTransactionDetail.FindAsync(id);
            if (propertyTransactionDetail != null)
            {
                _context.PropertyTransactionDetail.Remove(propertyTransactionDetail);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyTransactionDetailExists(int id)
        {
            return _context.PropertyTransactionDetail.Any(e => e.PropertyTransactionDetailID == id);
        }
    }
}
