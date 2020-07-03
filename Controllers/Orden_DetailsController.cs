using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmallWarehouseBackEnd.Contexts;
using SmallWarehouseBackEnd.Models;

namespace SmallWarehouseBackEnd.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class Orden_DetailsController : ControllerBase
    {
        private readonly AppDbContext _context;

        public Orden_DetailsController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orden_Details
        [HttpGet]
        public async Task<ActionResult<IEnumerable<Orden_Details>>> GetOrden_Details()
        {
            return await _context.Orden_Details.ToListAsync();
        }

        // GET: api/Orden_Details/5
        [HttpGet("{id}")]
        public async Task<ActionResult<Orden_Details>> GetOrden_Details(int id)
        {
            var orden_Details = await _context.Orden_Details.FindAsync(id);

            if (orden_Details == null)
            {
                return NotFound();
            }

            return orden_Details;
        }

        // PUT: api/Orden_Details/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPut("{id}")]
        public async Task<IActionResult> PutOrden_Details(int id, Orden_Details orden_Details)
        {
            if (id != orden_Details.orden_details_id)
            {
                return BadRequest();
            }

            _context.Entry(orden_Details).State = EntityState.Modified;

            try
            {
                await _context.SaveChangesAsync();
            }
            catch (DbUpdateConcurrencyException)
            {
                if (!Orden_DetailsExists(id))
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

        // POST: api/Orden_Details
        // To protect from overposting attacks, enable the specific properties you want to bind to, for
        // more details, see https://go.microsoft.com/fwlink/?linkid=2123754.
        [HttpPost]
        public async Task<ActionResult<Orden_Details>> PostOrden_Details(Orden_Details orden_Details)
        {
            _context.Orden_Details.Add(orden_Details);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden_Details", new { id = orden_Details.orden_details_id }, orden_Details);
        }

        // DELETE: api/Orden_Details/5
        [HttpDelete("{id}")]
        public async Task<ActionResult<Orden_Details>> DeleteOrden_Details(int id)
        {
            var orden_Details = await _context.Orden_Details.FindAsync(id);
            if (orden_Details == null)
            {
                return NotFound();
            }

            _context.Orden_Details.Remove(orden_Details);
            await _context.SaveChangesAsync();

            return orden_Details;
        }

        private bool Orden_DetailsExists(int id)
        {
            return _context.Orden_Details.Any(e => e.orden_details_id == id);
        }
    }
}
