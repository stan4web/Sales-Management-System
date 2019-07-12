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
    public class MyProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: MyProducts
        public async Task<IActionResult> Index()
        {
            return View(await _context.MyProducts.ToListAsync());
        }

        // GET: MyProducts/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProduct = await _context.MyProducts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (myProduct == null)
            {
                return NotFound();
            }

            return View(myProduct);
        }

        // GET: MyProducts/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MyProducts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Name,Quantity,Price,CreatedDate")] MyProduct myProduct)
        {
            if (ModelState.IsValid)
            {
                myProduct.CreatedDate = DateTime.Now;
                _context.Add(myProduct);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(myProduct);
        }

        // GET: MyProducts/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProduct = await _context.MyProducts.SingleOrDefaultAsync(m => m.Id == id);
            if (myProduct == null)
            {
                return NotFound();
            }
            return View(myProduct);
        }

        // POST: MyProducts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Name,Quantity,Price,CreatedDate")] MyProduct myProduct)
        {
            if (id != myProduct.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(myProduct);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MyProductExists(myProduct.Id))
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
            return View(myProduct);
        }

        // GET: MyProducts/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var myProduct = await _context.MyProducts
                .SingleOrDefaultAsync(m => m.Id == id);
            if (myProduct == null)
            {
                return NotFound();
            }

            return View(myProduct);
        }

        // POST: MyProducts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var myProduct = await _context.MyProducts.SingleOrDefaultAsync(m => m.Id == id);
            _context.MyProducts.Remove(myProduct);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool MyProductExists(int id)
        {
            return _context.MyProducts.Any(e => e.Id == id);
        }
    }
}
