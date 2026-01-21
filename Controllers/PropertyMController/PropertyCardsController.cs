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
    public class PropertyCardsController : Controller
    {
        private readonly SIETEContext _context;

        public PropertyCardsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: PropertyCards
        public async Task<IActionResult> Index()
        {
            return View(await _context.PropertyCard.ToListAsync());
        }

        // GET: PropertyCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCard = await _context.PropertyCard.Include(p => p.Properties).ThenInclude(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.PropertyCardID == id);
            if (propertyCard == null)
            {
                return NotFound();
            }

            return PartialView("Details", propertyCard);
        }

        // GET: PropertyCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: PropertyCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyCardID,PropertyCardName,PropertyDescription,PropertyUnitMeasurement,CurrentStockQuantity,TotalAmount,LastUpdated")] PropertyCard propertyCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(propertyCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(propertyCard);
        }

        // GET: PropertyCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCard = await _context.PropertyCard.FindAsync(id);
            if (propertyCard == null)
            {
                return NotFound();
            }
            return View(propertyCard);
        }

        // POST: PropertyCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyCardID,PropertyCardName,PropertyDescription,PropertyUnitMeasurement,CurrentStockQuantity,TotalAmount,LastUpdated")] PropertyCard propertyCard)
        {
            if (id != propertyCard.PropertyCardID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(propertyCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyCardExists(propertyCard.PropertyCardID))
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
            return View(propertyCard);
        }

        // GET: PropertyCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var propertyCard = await _context.PropertyCard
                .FirstOrDefaultAsync(m => m.PropertyCardID == id);
            if (propertyCard == null)
            {
                return NotFound();
            }

            return View(propertyCard);
        }

        // POST: PropertyCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var propertyCard = await _context.PropertyCard.FindAsync(id);
            if (propertyCard != null)
            {
                _context.PropertyCard.Remove(propertyCard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyCardExists(int id)
        {
            return _context.PropertyCard.Any(e => e.PropertyCardID == id);
        }
    }
}
