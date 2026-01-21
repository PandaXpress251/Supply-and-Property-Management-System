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
    public class PropertyAssignmentsController : Controller
    {
        private readonly SIETEContext _context;

        public PropertyAssignmentsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PropertyAssignments
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.PropertyAssignment.Include(p => p.Employee).Include(p => p.Property);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: PropertyAssignments/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignment = await _context.PropertyAssignment
                .Include(p => p.Employee)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.PropertyAssignmentID == id);
            if (propertyAssignment == null)
            {
                return NotFound();
            }

            return View(propertyAssignment);
        }

        // GET: PropertyAssignments/Create
        public IActionResult Create()
        {
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress");
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category");
            return View();
        }

        // POST: PropertyAssignments/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyAssignmentID,PropertyID,EmployeeID,DateAssigned,Location,Remarks,Quantity")] PropertyAssignment propertyAssignment)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyAssignment);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignment.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignment.PropertyID);
            return View(propertyAssignment);
        }

        // GET: PropertyAssignments/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignment = await _context.PropertyAssignment.FindAsync(id);
            if (propertyAssignment == null)
            {
                return NotFound();
            }
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignment.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignment.PropertyID);
            return View(propertyAssignment);
        }

        // POST: PropertyAssignments/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyAssignmentID,PropertyID,EmployeeID,DateAssigned,Location,Remarks,Quantity")] PropertyAssignment propertyAssignment)
        {
            if (id != propertyAssignment.PropertyAssignmentID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyAssignment);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyAssignmentExists(propertyAssignment.PropertyAssignmentID))
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
            ViewData["EmployeeID"] = new SelectList(_context.Employee, "EmployeeID", "EmailAddress", propertyAssignment.EmployeeID);
            ViewData["PropertyID"] = new SelectList(_context.Property, "PropertyID", "Category", propertyAssignment.PropertyID);
            return View(propertyAssignment);
        }

        // GET: PropertyAssignments/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyAssignment = await _context.PropertyAssignment
                .Include(p => p.Employee)
                .Include(p => p.Property)
                .FirstOrDefaultAsync(m => m.PropertyAssignmentID == id);
            if (propertyAssignment == null)
            {
                return NotFound();
            }

            return View(propertyAssignment);
        }

        // POST: PropertyAssignments/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyAssignment = await _context.PropertyAssignment.FindAsync(id);
            if (propertyAssignment != null)
            {
                _context.PropertyAssignment.Remove(propertyAssignment);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyAssignmentExists(int id)
        {
            return _context.PropertyAssignment.Any(e => e.PropertyAssignmentID == id);
        }
    }
}
