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
    public class PropertiesController : Controller
    {
        private readonly SIETEContext _context;

        public PropertiesController(SIETEContext context)
        {
            _context = context;
        }

        // GET: Properties
        public async Task<IActionResult> Index()
        {
            var sIETEContext = _context.Property.Include(f => f.FundCluster).Include(p => p.PropertyCard).Include(s => s.Supplier);
            return View(await sIETEContext.ToListAsync());
        }

        // GET: Properties/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Property
                .Include(f => f.FundCluster)
                .Include(p => p.PropertyCard)
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.PropertyID == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // GET: Properties/Create
        public IActionResult Create()
        {
            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode");
            ViewData["PropertyCardID"] = new SelectList(_context.Set<PropertyCard>(), "PropertyCardID", "PropertyCardName");
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Address");
            return View();
        }

        // POST: Properties/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("PropertyID,SupplierID,FundClusterID,PropertyCardID,StockPropNo,PropertyName,Description,UnitMeasurement,StockQuantity,UnitCost,DateAcquired,Status,Category")] Property @property)
        {
            if (ModelState.IsValid)
            {
                _context.Add(@property);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode", @property.FundClusterID);
            ViewData["PropertyCardID"] = new SelectList(_context.Set<PropertyCard>(), "PropertyCardID", "PropertyCardName", @property.PropertyCardID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Address", @property.SupplierID);
            return View(@property);
        }

        // GET: Properties/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Property.FindAsync(id);
            if (@property == null)
            {
                return NotFound();
            }
            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode", @property.FundClusterID);
            ViewData["PropertyCardID"] = new SelectList(_context.Set<PropertyCard>(), "PropertyCardID", "PropertyCardName", @property.PropertyCardID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Address", @property.SupplierID);
            return View(@property);
        }

        // POST: Properties/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("PropertyID,SupplierID,FundClusterID,PropertyCardID,StockPropNo,PropertyName,Description,UnitMeasurement,StockQuantity,UnitCost,DateAcquired,Status,Category")] Property @property)
        {
            if (id != @property.PropertyID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(@property);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!PropertyExists(@property.PropertyID))
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
            ViewData["FundClusterID"] = new SelectList(_context.FundCluster, "FundClusterID", "FundClusterCode", @property.FundClusterID);
            ViewData["PropertyCardID"] = new SelectList(_context.Set<PropertyCard>(), "PropertyCardID", "PropertyCardName", @property.PropertyCardID);
            ViewData["SupplierID"] = new SelectList(_context.Supplier, "SupplierID", "Address", @property.SupplierID);
            return View(@property);
        }

        // GET: Properties/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var @property = await _context.Property
                .Include(f => f.FundCluster)
                .Include(p => p.PropertyCard)
                .Include(s => s.Supplier)
                .FirstOrDefaultAsync(m => m.PropertyID == id);
            if (@property == null)
            {
                return NotFound();
            }

            return View(@property);
        }

        // POST: Properties/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var @property = await _context.Property.FindAsync(id);
            if (@property != null)
            {
                _context.Property.Remove(@property);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool PropertyExists(int id)
        {
            return _context.Property.Any(e => e.PropertyID == id);
        }
    }
}
