using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesApp.Data;
using SalesApp.Models;

namespace SalesApp.Controllers
{
    public class MySalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MySalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MySales
        public async Task<IActionResult> Index()
        {
            return View(await _context.MySales.ToListAsync());
        }

        // GET: MySales/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mySale = await _context.MySales
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mySale == null)
            {
                return NotFound();
            }

            return View(mySale);
        }

        // GET: MySales/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MySales/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ProductName,Price,Quantity")] MySale mySale)
        {
            if (ModelState.IsValid)
            {
                _context.Add(mySale);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(mySale);
        }

        // GET: MySales/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mySale = await _context.MySales.SingleOrDefaultAsync(m => m.Id == id);
            if (mySale == null)
            {
                return NotFound();
            }
            return View(mySale);
        }

        // POST: MySales/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ProductName,Price,Quantity")] MySale mySale)
        {
            if (id != mySale.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(mySale);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MySaleExists(mySale.Id))
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
            return View(mySale);
        }

        // GET: MySales/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var mySale = await _context.MySales
                .SingleOrDefaultAsync(m => m.Id == id);
            if (mySale == null)
            {
                return NotFound();
            }

            return View(mySale);
        }

        // POST: MySales/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var mySale = await _context.MySales.SingleOrDefaultAsync(m => m.Id == id);
            _context.MySales.Remove(mySale);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MySaleExists(int id)
        {
            return _context.MySales.Any(e => e.Id == id);
        }
    }
}
