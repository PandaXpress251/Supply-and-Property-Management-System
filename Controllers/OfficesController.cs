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
    
    public class OfficesController : Controller
    {
        private readonly SIETEContext _context;

        public OfficesController(SIETEContext context) => _context = context;

        public async Task<IActionResult> Index() => 
            View(await _context.Office.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var office = await _context.Office
                .Include(o => o.Employees.Where(e => e.IsActive))
                .FirstOrDefaultAsync(m => m.OfficeID == id);

            return office == null ? NotFound() : PartialView(office);
        }

        public IActionResult Create() => PartialView();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Office office)
        {
            if (!ModelState.IsValid) return PartialView(office);

            office.OfficeName = office.OfficeName?.Trim().ToUpper();
            office.Acronym = office.Acronym?.Trim().ToUpper();

            if (await _context.Office.AnyAsync(o => o.OfficeName.Trim().ToUpper() == office.OfficeName))
            {
                ModelState.AddModelError("OfficeName", "This office name already exists.");
                return PartialView(office);
            }

            if (await _context.Office.AnyAsync(o => o.RespCenter_Code == office.RespCenter_Code))
            {
                ModelState.AddModelError("RespCenter_Code", "This responsibility center code already exists.");
                return PartialView(office);
            }

            try
            {
                _context.Add(office);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Office created successfully.", office = office });
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the office. Please try again.");
                return PartialView(office);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var office = await _context.Office.FindAsync(id);
            return office == null ? NotFound() : PartialView(office);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Office office)
        {
            if (id != office.OfficeID) return NotFound();
            if (!ModelState.IsValid) return PartialView(office);

            try
            {
                var existingOffice = await _context.Office
                    .Include(o => o.Employees)
                    .FirstOrDefaultAsync(o => o.OfficeID == id);

                if (existingOffice == null) return NotFound();

                if (!office.IsActive && existingOffice.Employees.Any(e => e.IsActive))
                {
                    ModelState.AddModelError("IsActive", "Cannot deactivate office because it has active employees.");
                    return PartialView(office);
                }

                office.OfficeName = office.OfficeName?.Trim().ToUpper();
                office.Acronym = office.Acronym?.Trim().ToUpper();

                if (await _context.Office.AnyAsync(o => 
                    o.OfficeID != id && 
                    o.OfficeName.Trim().ToUpper() == office.OfficeName))
                {
                    ModelState.AddModelError("OfficeName", "This office name already exists.");
                    return PartialView(office);
                }

                if (await _context.Office.AnyAsync(o =>
                    o.OfficeID != id &&
                    o.Acronym.Trim().ToUpper() == office.Acronym))
                {
                    ModelState.AddModelError("Acronym", "This Acronym already exists.");
                    return PartialView(office);
                }

                if (await _context.Office.AnyAsync(o => 
                    o.OfficeID != id && 
                    o.RespCenter_Code == office.RespCenter_Code))
                {
                    ModelState.AddModelError("RespCenter_Code", "This responsibility center code already exists.");
                    return PartialView(office);
                }

                existingOffice.OfficeName = office.OfficeName;
                existingOffice.Acronym = office.Acronym;
                existingOffice.OfficeType = office.OfficeType;
                existingOffice.RespCenter_Code = office.RespCenter_Code;
                existingOffice.Parent_Code = office.Parent_Code;
                existingOffice.IsActive = office.IsActive;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Office updated successfully."});
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!OfficeExists(office.OfficeID))
                    return NotFound();
                
                ModelState.AddModelError("", "The record was modified by another user. Please refresh and try again.");
                return PartialView(office);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the office. Please try again.");
                return PartialView(office);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoSave(int id, [FromBody] Office office)
        {
            if (id != office.OfficeID) return NotFound();

            try
            {
                var existingOffice = await _context.Office
                    .Include(o => o.Employees)
                    .FirstOrDefaultAsync(o => o.OfficeID == id);

                if (existingOffice == null) return NotFound();

                // Only update specific fields for auto-save
                existingOffice.OfficeName = office.OfficeName?.Trim().ToUpper();
                existingOffice.Acronym = office.Acronym?.Trim().ToUpper();
                existingOffice.OfficeType = office.OfficeType;
                existingOffice.RespCenter_Code = office.RespCenter_Code;
                existingOffice.Parent_Code = office.Parent_Code;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Changes saved automatically.", office = existingOffice });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to save changes automatically." });
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var office = await _context.Office
                .Include(o => o.Employees)
                .FirstOrDefaultAsync(m => m.OfficeID == id);

            return office == null ? NotFound() : PartialView(office);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var office = await _context.Office
                .Include(o => o.Employees)
                .FirstOrDefaultAsync(o => o.OfficeID == id);

            if (office == null)
                return Json(new { success = false, message = "Office not found." });

            if (office.Employees.Any(e => e.IsActive))
                return Json(new { success = false, message = "Cannot deactivate office because it still has active employees." });

            try
            {
                office.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Office deactivated successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred while deactivating the office. Please try again." });
            }
        }

        private bool OfficeExists(int id) => _context.Office.Any(e => e.OfficeID == id);
    }
}
