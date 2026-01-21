using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSCRO7_SPMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models;

namespace SIETE.Controllers
{
    [AdminOnly]
    public class EmployeesController : Controller
    {
        private readonly SIETEContext _context;

        public EmployeesController(SIETEContext context)
        {
            _context = context;
        }

        // GET: Employees
        public async Task<IActionResult> Index()
        {
            var employees = await _context.Employee
                .Include(e => e.Office)
                .Include(e => e.PlantillaPosition)
                .ToListAsync();
            return View(employees);
        }

        // GET: Employees/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Employee ID is required." });
            }

            var employee = await _context.Employee
                .Include(e => e.Office)
                .Include(e => e.PlantillaPosition)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);

            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found." });
            }

            return PartialView(employee);
        }

        // GET: Employees/Create
        public IActionResult Create()
        {
            ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
            ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
            return PartialView();
        }

        // POST: Employees/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Title,FirstName,LastName,Suffix,OfficeID,PositionID,EmailAddress,PhoneNumber,IsActive,IsAccountablePerson")] Employee employee)
        {
            try
            {
                // Validate Title length
                if (!string.IsNullOrEmpty(employee.Title) && employee.Title.Length > 20)
                {
                    ModelState.AddModelError("Title", "Title cannot exceed 20 characters.");
                }

                // Validate Suffix length
                if (!string.IsNullOrEmpty(employee.Suffix) && employee.Suffix.Length > 10)
                {
                    ModelState.AddModelError("Suffix", "Suffix cannot exceed 10 characters.");
                }

                // Check if email already exists
                if (await _context.Employee.AnyAsync(e => e.EmailAddress == employee.EmailAddress))
                {
                    ModelState.AddModelError("EmailAddress", "This email address is already registered.");
                }

                // Check if phone number already exists
                if (await _context.Employee.AnyAsync(e => e.PhoneNumber == employee.PhoneNumber))
                {
                    ModelState.AddModelError("PhoneNumber", "This phone number is already registered.");
                }

                // Check if office exists and is active (only if OfficeID is provided)
                if (employee.OfficeID.HasValue)
                {
                    var office = await _context.Office.FindAsync(employee.OfficeID.Value);
                    if (office == null || !office.IsActive)
                    {
                        ModelState.AddModelError("OfficeID", "Selected office is not valid or inactive.");
                    }
                }

                // Check if position exists and is active (only if PositionID is provided)
                if (employee.PositionID.HasValue)
                {
                    var position = await _context.PlantillaPosition.FindAsync(employee.PositionID.Value);
                    if (position == null || !position.IsActive)
                    {
                        ModelState.AddModelError("PositionID", "Selected position is not valid or inactive.");
                    }
                }

                if (ModelState.IsValid)
                {
                    _context.Add(employee);
                    await _context.SaveChangesAsync();
                    return Json(new { success = true, message = "Employee created successfully." });
                }
            }
            catch (DbUpdateException ex)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
                // Log the exception details here
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", "An unexpected error occurred. Please try again.");
                // Log the exception details here
            }

            ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
            ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
            return PartialView(employee);
        }

        // GET: Employees/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return Json(new { success = false, message = "Employee ID is required." });
            }

            var employee = await _context.Employee
                .Include(e => e.Office)
                .Include(e => e.PlantillaPosition)
                .FirstOrDefaultAsync(e => e.EmployeeID == id);
                
            if (employee == null)
            {
                return Json(new { success = false, message = "Employee not found." });
            }

            ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
            ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
            return PartialView(employee);
        }

        // POST: Employees/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Employee employee)
        {
            if (id != employee.EmployeeID)
            {
                return Json(new { success = false, message = "Invalid employee ID." });
            }

            try
            {
                if (ModelState.IsValid)
                {
                    // Get the existing employee from database
                    var existingEmployee = await _context.Employee
                        .Include(e => e.Office)
                        .Include(e => e.PlantillaPosition)
                        .FirstOrDefaultAsync(e => e.EmployeeID == id);

                    if (existingEmployee == null)
                    {
                        return Json(new { success = false, message = "Employee not found." });
                    }

                    // Handle inactive employee case
                    if (employee.IsActive == false)
                    {
                        employee.OfficeID = null;
                        employee.PositionID = null;
                    }
                    else
                    {
                        // Check if office exists and is active (only if OfficeID is provided)
                        if (employee.OfficeID.HasValue)
                        {
                            var office = await _context.Office.FindAsync(employee.OfficeID.Value);
                            if (office == null || !office.IsActive)
                            {
                                ModelState.AddModelError("OfficeID", "Selected office is not valid or inactive.");
                                ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
                                ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
                                return PartialView(employee);
                            }
                        }

                        // Check if position exists and is active (only if PositionID is provided)
                        if (employee.PositionID.HasValue)
                        {
                            var position = await _context.PlantillaPosition.FindAsync(employee.PositionID.Value);
                            if (position == null || !position.IsActive)
                            {
                                ModelState.AddModelError("PositionID", "Selected position is not valid or inactive.");
                                ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
                                ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
                                return PartialView(employee);
                            }
                        }
                    }

                    // Update the existing employee's properties
                    existingEmployee.Title = employee.Title;
                    existingEmployee.FirstName = employee.FirstName;
                    existingEmployee.LastName = employee.LastName;
                    existingEmployee.Suffix = employee.Suffix;
                    existingEmployee.EmailAddress = employee.EmailAddress;
                    existingEmployee.PhoneNumber = employee.PhoneNumber;
                    existingEmployee.IsActive = employee.IsActive;
                    existingEmployee.IsAccountablePerson = employee.IsAccountablePerson;
                    existingEmployee.OfficeID = employee.OfficeID;
                    existingEmployee.PositionID = employee.PositionID;

                    await _context.SaveChangesAsync();
                    
                    return Json(new { success = true, message = "Employee updated successfully." });
                }
            }
            catch (DbUpdateException)
            {
                ModelState.AddModelError("", "Unable to save changes. Please try again.");
            }

            ViewBag.Offices = _context.Office.Where(o => o.IsActive).ToList();
            ViewBag.Positions = _context.PlantillaPosition.Where(p => p.IsActive).ToList();
            return PartialView(employee);
        }

        // GET: Employees/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var employee = await _context.Employee
                .Include(e => e.Office)
                .Include(e => e.PlantillaPosition)
                .FirstOrDefaultAsync(m => m.EmployeeID == id);
            if (employee == null)
            {
                return NotFound();
            }

            return PartialView(employee);
        }

        // POST: Employees/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var employee = await _context.Employee.FindAsync(id);
            if (employee != null)
            {
                _context.Employee.Remove(employee);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmployeeExists(int id)
        {
            return _context.Employee.Any(e => e.EmployeeID == id);
        }
    }
}
