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
    public class PlantillaPositionsController : Controller
    {
        private readonly SIETEContext _context;

        public PlantillaPositionsController(SIETEContext context)
        {
            _context = context;
        }

        public async Task<IActionResult> Index() => 
            View(await _context.PlantillaPosition.ToListAsync());

        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var position = await _context.PlantillaPosition
                .Include(p => p.Employees.Where(e => e.IsActive)).ThenInclude(o=>o.Office)
                .FirstOrDefaultAsync(m => m.PositionID == id);

            return position == null ? NotFound() : PartialView(position);
        }

        public IActionResult Create() => PartialView();

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PlantillaPosition position)
        {
            if (!ModelState.IsValid) return PartialView(position);

            position.PositionTitle = position.PositionTitle?.Trim().ToUpper();

            if (await _context.PlantillaPosition.AnyAsync(p => p.PositionTitle.Trim().ToUpper() == position.PositionTitle))
            {
                ModelState.AddModelError("PositionTitle", "This position title already exists.");
                return PartialView(position);
            }

            try
            {
                _context.Add(position);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Position created successfully.", position = position });
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the position. Please try again.");
                return PartialView(position);
            }
        }

        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var position = await _context.PlantillaPosition.FindAsync(id);
            return position == null ? NotFound() : PartialView(position);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, PlantillaPosition position)
        {
            if (id != position.PositionID) return NotFound();
            if (!ModelState.IsValid) return PartialView(position);

            try
            {
                var existingPosition = await _context.PlantillaPosition
                    .Include(p => p.Employees)
                    .FirstOrDefaultAsync(p => p.PositionID == id);

                if (existingPosition == null) return NotFound();

                if (!position.IsActive && existingPosition.Employees.Any(e => e.IsActive))
                {
                    ModelState.AddModelError("IsActive", "Cannot deactivate position because it has active employees.");
                    return PartialView(position);
                }

                position.PositionTitle = position.PositionTitle?.Trim().ToUpper();

                if (await _context.PlantillaPosition.AnyAsync(p => 
                    p.PositionID != id && 
                    p.PositionTitle.Trim().ToUpper() == position.PositionTitle))
                {
                    ModelState.AddModelError("PositionTitle", "This position title already exists.");
                    return PartialView(position);
                }

                existingPosition.PositionTitle = position.PositionTitle;
                existingPosition.IsActive = position.IsActive;

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Position updated successfully.", position = existingPosition });
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!PositionExists(position.PositionID))
                    return NotFound();
                
                ModelState.AddModelError("", "The record was modified by another user. Please refresh and try again.");
                return PartialView(position);
            }
            catch
            {
                ModelState.AddModelError("", "An error occurred while saving the position. Please try again.");
                return PartialView(position);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AutoSave(int id, [FromBody] PlantillaPosition position)
        {
            if (id != position.PositionID) return NotFound();

            try
            {
                var existingPosition = await _context.PlantillaPosition
                    .Include(p => p.Employees)
                    .FirstOrDefaultAsync(p => p.PositionID == id);

                if (existingPosition == null) return NotFound();

                // Only update specific fields for auto-save
                existingPosition.PositionTitle = position.PositionTitle?.Trim().ToUpper();

                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Changes saved automatically.", position = existingPosition });
            }
            catch
            {
                return Json(new { success = false, message = "Failed to save changes automatically." });
            }
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var position = await _context.PlantillaPosition
                .FirstOrDefaultAsync(m => m.PositionID == id);

            return position == null ? NotFound() : PartialView(position);
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var position = await _context.PlantillaPosition
                .Include(p => p.Employees)
                .FirstOrDefaultAsync(p => p.PositionID == id);

            if (position == null)
                return Json(new { success = false, message = "Position not found." });

            if (position.Employees.Any(e => e.IsActive))
                return Json(new { success = false, message = "Cannot deactivate position because it still has active employees." });

            try
            {
                position.IsActive = false;
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Position deactivated successfully." });
            }
            catch
            {
                return Json(new { success = false, message = "An error occurred while deactivating the position. Please try again." });
            }
        }

        private bool PositionExists(int id) => _context.PlantillaPosition.Any(e => e.PositionID == id);
    }
}

