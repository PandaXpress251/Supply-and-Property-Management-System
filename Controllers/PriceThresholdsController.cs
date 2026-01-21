using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CSCRO7_SPMS.Helpers;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.CodeAnalysis.Elfie.Diagnostics;
using Microsoft.EntityFrameworkCore;
using SIETE.Data;
using SIETE.Models;

namespace SIETE.Controllers
{
    [AdminOnly]
    public class PriceThresholdsController : Controller
    {
        private readonly SIETEContext _context;

        public PriceThresholdsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PriceThresholds
        public async Task<IActionResult> Index()
        {
            return View(await _context.PriceThreshold.ToListAsync());
        }

        // GET: PriceThresholds/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceThreshold = await _context.PriceThreshold
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceThreshold == null)
            {
                return NotFound();
            }

            return View(priceThreshold);
        }

        // GET: PriceThresholds/Create
        public IActionResult Create()
        {
            return PartialView();
        }

        // POST: PriceThresholds/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(PriceThreshold priceThreshold)
        {
           
                _context.Add(priceThreshold);
                await _context.SaveChangesAsync();
                return Json(new { success = true, message = "Position created successfully." });
            
            
        }

        // GET: PriceThresholds/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceThreshold = await _context.PriceThreshold.FindAsync(id);
            if (priceThreshold == null)
            {
                return NotFound();
            }
            return PartialView(priceThreshold);
        }

        // POST: PriceThresholds/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,PropertyThreshold,SPThreshold,EffectiveDate")] PriceThreshold priceThreshold)
        {
            if (id != priceThreshold.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    priceThreshold.EffectiveDate = DateTime.UtcNow;
                    _context.Update(priceThreshold);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PriceThresholdExists(priceThreshold.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return Json(new { success = true, message = " Updated successfully." });
            }
            return PartialView(priceThreshold);
        }

        // GET: PriceThresholds/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var priceThreshold = await _context.PriceThreshold
                .FirstOrDefaultAsync(m => m.Id == id);
            if (priceThreshold == null)
            {
                return NotFound();
            }

            return View(priceThreshold);
        }

        // POST: PriceThresholds/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var priceThreshold = await _context.PriceThreshold.FindAsync(id);
            if (priceThreshold != null)
            {
                _context.PriceThreshold.Remove(priceThreshold);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PriceThresholdExists(int id)
        {
            return _context.PriceThreshold.Any(e => e.Id == id);
        }
    }
}
