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
    [Route("api/MySales")]
    public class MySalesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public MySalesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: api/MySales
        [HttpGet]
        public IEnumerable<MySale> GetMySales()
        {
            return _context.MySales;
        }

        // GET: api/MySales/5
        [HttpGet("{id}")]
        public async Task<IActionResult> GetMySale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mySale = await _context.MySales.SingleOrDefaultAsync(m => m.Id == id);

            if (mySale == null)
            {
                return NotFound();
            }

            return Ok(mySale);
        }

        // PUT: api/MySales/5
        [HttpPut("{id}")]
        public async Task<IActionResult> PutMySale([FromRoute] int id, [FromBody] MySale mySale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            if (id != mySale.Id)
            {
                return BadRequest();
            }

            _context.Entry(mySale).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!MySaleExists(id))
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

        // POST: api/MySales
        [HttpPost]
        public async Task<IActionResult> PostMySale([FromBody] MySale mySale)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            _context.MySales.Add(mySale);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetMySale", new { id = mySale.Id }, mySale);
        }

        // DELETE: api/MySales/5
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteMySale([FromRoute] int id)
        {
            if (!ModelState.IsValid)
            {
                return BadRequest(ModelState);
            }

            var mySale = await _context.MySales.SingleOrDefaultAsync(m => m.Id == id);
            if (mySale == null)
            {
                return NotFound();
            }

            _context.MySales.Remove(mySale);
            await _context.SaveChangesAsync();

            return Ok(mySale);
        }

        private bool MySaleExists(int id)
        {
            return _context.MySales.Any(e => e.Id == id);
        }
    }
}