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
    public class StockCardsController : Controller
    {
        private readonly SIETEContext _context;

        public StockCardsController(SIETEContext context)
        {
            _context = context;
        }

        // GET: StockCards
        public async Task<IActionResult> Index()
        {
            return View(await _context.StockCard.ToListAsync());
        }

        // GET: StockCards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockCard = await _context.StockCard.Include(s => s.Supplies).ThenInclude(sp => sp.Supplier)
                .FirstOrDefaultAsync(m => m.StockCardID == id);
            if (stockCard == null)
            {
                return NotFound();
            }

            return PartialView("Details", stockCard);
        }

        // GET: StockCards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: StockCards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("StockCardID,StockCardName,StockDescription,StockUnitMeasurement,CurrentStockQuantity,TotalAmount,LastUpdated")] StockCard stockCard)
        {
            if (ModelState.IsValid)
            {
                _context.Add(stockCard);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(stockCard);
        }

        // GET: StockCards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockCard = await _context.StockCard.FindAsync(id);
            if (stockCard == null)
            {
                return NotFound();
            }
            return View(stockCard);
        }

        // POST: StockCards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("StockCardID,StockCardName,StockDescription,StockUnitMeasurement,CurrentStockQuantity,TotalAmount,LastUpdated")] StockCard stockCard)
        {
            if (id != stockCard.StockCardID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(stockCard);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!StockCardExists(stockCard.StockCardID))
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
            return View(stockCard);
        }

        // GET: StockCards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var stockCard = await _context.StockCard
                .FirstOrDefaultAsync(m => m.StockCardID == id);
            if (stockCard == null)
            {
                return NotFound();
            }

            return View(stockCard);
        }

        // POST: StockCards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var stockCard = await _context.StockCard.FindAsync(id);
            if (stockCard != null)
            {
                _context.StockCard.Remove(stockCard);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool StockCardExists(int id)
        {
            return _context.StockCard.Any(e => e.StockCardID == id);
        }
    }
}
