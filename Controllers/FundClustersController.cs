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
    public class FundClustersController : Controller
    {
        private readonly SIETEContext _context;

        public FundClustersController(SIETEContext context)
        {
            _context = context;
        }

        // GET: FundClusters
        public async Task<IActionResult> Index()
        {
            return View(await _context.FundCluster.ToListAsync());
        }

        // GET: FundClusters/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundCluster = await _context.FundCluster
                .FirstOrDefaultAsync(m => m.FundClusterID == id);
            if (fundCluster == null)
            {
                return NotFound();
            }

            return View(fundCluster);
        }

        // GET: FundClusters/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: FundClusters/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("FundClusterID,FundClusterCode,FundClusterName")] FundCluster fundCluster)
        {
            if (ModelState.IsValid)
            {
                _context.Add(fundCluster);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(fundCluster);
        }

        // GET: FundClusters/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundCluster = await _context.FundCluster.FindAsync(id);
            if (fundCluster == null)
            {
                return NotFound();
            }
            return View(fundCluster);
        }

        // POST: FundClusters/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("FundClusterID,FundClusterCode,FundClusterName")] FundCluster fundCluster)
        {
            if (id != fundCluster.FundClusterID)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(fundCluster);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!FundClusterExists(fundCluster.FundClusterID))
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
            return View(fundCluster);
        }

        // GET: FundClusters/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var fundCluster = await _context.FundCluster
                .FirstOrDefaultAsync(m => m.FundClusterID == id);
            if (fundCluster == null)
            {
                return NotFound();
            }

            return View(fundCluster);
        }

        // POST: FundClusters/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var fundCluster = await _context.FundCluster.FindAsync(id);
            if (fundCluster != null)
            {
                _context.FundCluster.Remove(fundCluster);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool FundClusterExists(int id)
        {
            return _context.FundCluster.Any(e => e.FundClusterID == id);
        }
    }
}
