using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SalesApp.Data;
using SalesApp.Models;

namespace SalesApp.Controllers.Api
{
    [Produces("application/json")]
    [Route("api/MyProducts")]
    public class MyProductsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MyProductsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MyProducts
        [HttpGet]
        public IEnumerable<MyProduct> GetMyProducts()
        {
            return _context.MyProducts;
        }

        // GET: api/MyProducts/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMyProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var myProduct = await _context.MyProducts.SingleOrDefaultAsync(m => m.Id == id);

            if (myProduct == null)
            {
                return NotFound();
            }

            return Ok(myProduct);
        }

        // PUT: api/MyProducts/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMyProduct([FromRoute] int id, [FromBody] MyProduct myProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != myProduct.Id)
            {
                return BadRequest();
            }

            _context.Entry(myProduct).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MyProductExists(id))
                {
                    return NotFound();
                }
                else
                {
                    throw;
                }
            }

            return NoContent();
        }

        // POST: api/MyProducts
        [HttpPost]
        public async Task<IActionResult> PostMyProduct([FromBody] MyProduct myProduct)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MyProducts.Add(myProduct);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMyProduct", new { id = myProduct.Id }, myProduct);
        }

        // DELETE: api/MyProducts/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMyProduct([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var myProduct = await _context.MyProducts.SingleOrDefaultAsync(m => m.Id == id);
            if (myProduct == null)
            {
                return NotFound();
            }

            _context.MyProducts.Remove(myProduct);
            await _context.SaveChangesAsync();

            return Ok(myProduct);
        }

        private bool MyProductExists(int id)
        {
            return _context.MyProducts.Any(e => e.Id == id);
        }
    }
}