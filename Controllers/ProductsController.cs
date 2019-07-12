using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SalesApp.Data;
using SalesApp.Models;

namespace SalesApp.Controllers
{
    public class ProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public ProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Products
        public async Task<IActionResult> Index()
        {
            ViewBag.Product = "active";
            var applicationDbContext = _context.Products.OrderByDescending(v=>v.Id).Include(p => p.Category);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Products/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            ViewBag.Product = "active";
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // GET: Products/Create
        public IActionResult Create()
        {
            ViewBag.Product = "active";
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name");
            return View();
        }

        // POST: Products/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,CategoryId,Name,Quantity,PurchasePrice,ReOrderLevel,ExpirationDate,RegisteredDate,ImageUrl,ProductStatus,SubCategoryId,TotalPrice,SalingPrice")] Product product, IFormFile file, bool pstatus1)
        {
            ViewData["Parent"] = "active";
            System.Random random = new System.Random();
            int genNumber = random.Next(1234567890);

            var path = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot/Uploads/Products",
                           Path.GetFileName(genNumber + file.FileName));

            product.ImageUrl = path;

            using (var stream = new FileStream(path, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var getproduct = new Product
            {
                CategoryId = product.CategoryId,
                Name = product.Name,
                Quantity = product.Quantity,
                PurchasePrice = product.PurchasePrice,
                ReOrderLevel = product.ReOrderLevel,
                ExpirationDate = product.ExpirationDate,
                RegisteredDate = DateTime.Now,
                ImageUrl = path,
                ProductStatus = true,
                SubCategoryId = product.SubCategoryId,
                Description = product.Description,
                TotalPrice = product.Quantity * product.PurchasePrice,
                SalingPrice=product.SalingPrice

            };

            _context.Add(getproduct);
            await _context.SaveChangesAsync();
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return RedirectToAction(nameof(Index));
           
            //return View(product);
        }

        // GET: Products/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            ViewBag.Product = "active";
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // POST: Products/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,CategoryId,Name,Quantity,PurchasePrice,ReOrderLevel,ExpirationDate,RegisteredDate,ImageUrl,ProductStatus,SubCategoryId,SalingPrice")] Product product, IFormFile file)
        {
            ViewData["Parent"] = "active";
            System.Random random = new System.Random();
            int genNumber = random.Next(1234567890);

            

            if(file != null)
            {
                var path = Path.Combine(
                           Directory.GetCurrentDirectory(), "wwwroot/Uploads/Products",
                           Path.GetFileName(genNumber + file.FileName));

                product.ImageUrl = path;
                using (var stream = new FileStream(path, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

            }


            if (id != product.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(product);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!ProductExists(product.Id))
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
            ViewData["CategoryId"] = new SelectList(_context.Categories, "Id", "Name", product.CategoryId);
            return View(product);
        }

        // GET: Products/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            ViewBag.Product = "active";
            if (id == null)
            {
                return NotFound();
            }

            var product = await _context.Products
                .Include(p => p.Category)
                .SingleOrDefaultAsync(m => m.Id == id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: Products/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var product = await _context.Products.SingleOrDefaultAsync(m => m.Id == id);
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool ProductExists(int id)
        {
            return _context.Products.Any(e => e.Id == id);
        }

        [HttpGet]
        public JsonResult GetCategoryList(int categoryId)
        {
            var subcategories = new SelectList(_context.SubCategories.Where(c => c.Category.Id == categoryId), "Id", "Name");
            return Json(subcategories);

        }
    }
}
