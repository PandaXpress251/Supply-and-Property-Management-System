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
    public class PropertyAssignmentHistoriesController : Controller
    {
        private readonly SIETEContext _context;

        public PropertyAssignmentHistoriesController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PropertyAssignmentHistories
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.PropertyAssignmentHistory.Include(p => p.Employee).Include(p => p.Property).Include(p => p.PropertyTransaction);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: PropertyAssignmentHistories/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignmentHistory = await _context.PropertyAssignmentHistory
                .Include(p => p.Employee)
                .Include(p => p.Property)
                .Include(p => p.PropertyTransaction)
                .FirstOrDefaultAsync(m => m.HistoryID == id);
            if (propertyAssignmentHistory == null)
            {
                return NotFound();
            }

            return View(propertyAssignmentHistory);
        }

        // GET: PropertyAssignmentHistories/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress");
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category");
            ViewData["PropertyTransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID");
            return View();
        }

        // POST: PropertyAssignmentHistories/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("HistoryID,PropertyID,EmployeeID,StartDate,EndDate,Location,TPSNumber,TransferType,Remarks,PropertyTransactionID,Quantity")] PropertyAssignmentHistory propertyAssignmentHistory)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyAssignmentHistory);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignmentHistory.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignmentHistory.PropertyID);
            ViewData["PropertyTransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyAssignmentHistory.PropertyTransactionID);
            return View(propertyAssignmentHistory);
        }

        // GET: PropertyAssignmentHistories/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignmentHistory = await _context.PropertyAssignmentHistory.FindAsync(id);
            if (propertyAssignmentHistory == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignmentHistory.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignmentHistory.PropertyID);
            ViewData["PropertyTransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyAssignmentHistory.PropertyTransactionID);
            return View(propertyAssignmentHistory);
        }

        // POST: PropertyAssignmentHistories/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("HistoryID,PropertyID,EmployeeID,StartDate,EndDate,Location,TPSNumber,TransferType,Remarks,PropertyTransactionID,Quantity")] PropertyAssignmentHistory propertyAssignmentHistory)
        {
            if (id != propertyAssignmentHistory.HistoryID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyAssignmentHistory);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyAssignmentHistoryExists(propertyAssignmentHistory.HistoryID))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignmentHistory.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignmentHistory.PropertyID);
            ViewData["PropertyTransactionID"] = new SelectList(_context.PropertyTransaction, "TransactionID", "TransactionID", propertyAssignmentHistory.PropertyTransactionID);
            return View(propertyAssignmentHistory);
        }

        // GET: PropertyAssignmentHistories/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignmentHistory = await _context.PropertyAssignmentHistory
                .Include(p => p.Employee)
                .Include(p => p.Property)
                .Include(p => p.PropertyTransaction)
                .FirstOrDefaultAsync(m => m.HistoryID == id);
            if (propertyAssignmentHistory == null)
            {
                return NotFound();
            }

            return View(propertyAssignmentHistory);
        }

        // POST: PropertyAssignmentHistories/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyAssignmentHistory = await _context.PropertyAssignmentHistory.FindAsync(id);
            if (propertyAssignmentHistory != null)
            {
                _context.PropertyAssignmentHistory.Remove(propertyAssignmentHistory);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyAssignmentHistoryExists(int id)
        {
            return _context.PropertyAssignmentHistory.Any(e => e.HistoryID == id);
        }
    }
}
