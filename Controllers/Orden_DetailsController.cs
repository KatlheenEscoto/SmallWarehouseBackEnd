﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
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
        [Authorize]
        public async Task<ActionResult<IEnumerable<Orden_Details>>> GetOrden_Details()
        {
            return await _context.Orden_Details.ToListAsync();
        }

        // GET: api/Orden_Details/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Orden_Details>> GetOrden_Details(int id)
        {
            var orden_Details = await _context.Orden_Details.FindAsync(id);

            if (orden_Details == null)
            {
                return NotFound();
            }

            return orden_Details;
        }

        private bool Orden_DetailsExists(int id)
        {
            return _context.Orden_Details.Any(e => e.orden_details_id == id);
        }


        /*
         *       
         *
         * 
         * 
         */

        // Custom functions. c:

        // POST: api/Orden_Details
        // Agregar detalle de orden.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Orden_Details>> PostOrden_Details(Orden_Details orden_details)
        {
            // Verificando si existen las claves foraneas.
            var item = await _context.Item.FindAsync(orden_details.item_id);
            var orden = await _context.Orden.FindAsync(orden_details.orden_id);
            if (item == null || orden == null)
            {
                return NotFound("Error: La llave foranea a la que se hace referencia no existe. Revisar el item o la orden de referencia.");
            }
            if (orden_details.orden_details_qty <= 0) // Tiene que comprar algo y la orden debe estar en proceso (1).
            {
                return BadRequest("El mínimo de artículos es uno, si compra.");
            } else if ( item.item_qty < orden_details.orden_details_qty)
            {
                return BadRequest("Ha solicitado más artículos de los que se encuentran disponibles.");

            } else if ( orden.orden_status != 1 )
            {
                return BadRequest("La orden debe estar en proceso, no cancelada ni pagada para agregar otro artículo.");
            }
            orden_details.orden_details_status = 1; // En proceso de compra.
            item.item_qty = item.item_qty - orden_details.orden_details_qty; // Modificando el stock.
            _context.Orden_Details.Add(orden_details);

            // Log.
            Log log = new Log {
                log_date = DateTime.Today,
                orden_details_qty = orden_details.orden_details_qty,
                log_status = orden_details.orden_details_status,
                item_sku = item.item_sku,
                item_id = item.item_id,
                usuario_id = 1
            };
            _context.Log.Add(log);


            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden_Details", new { id = orden_details.orden_details_id }, orden_details);
        }


        // DELETE: api/Orden_Details/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Orden_Details>> DeleteOrden_Details(int id)
        {
            /* Cancelando una determinada orden de producto. */
            var orden_details_delete = await _context.Orden_Details.FindAsync(id);
            var item = await _context.Item.FindAsync(orden_details_delete.item_id);
            var orden = await _context.Orden.FindAsync(orden_details_delete.orden_id);

            if (orden_details_delete == null)
            {
                return NotFound("El detalle de orden no existe.");
            } 
            else if(orden.orden_status != 1)
            {
                return BadRequest("La orden debe estar en proceso, no cancelada ni pagada para cancelar artículo.");
            }

            item.item_qty = item.item_qty + orden_details_delete.orden_details_qty; // Modificando el stock.
            orden_details_delete.orden_details_status = 0; // Estado cancelada.

            try
            {
                // Log.
                Log log = new Log
                {
                    log_date = DateTime.Today,
                    orden_details_qty = orden_details_delete.orden_details_qty,
                    log_status = orden_details_delete.orden_details_status,
                    item_sku = item.item_sku,
                    item_id = item.item_id,
                    usuario_id = 1
                };
                _context.Log.Add(log);

                await _context.SaveChangesAsync();
                return orden_details_delete;
            }
            catch (DbUpdateConcurrencyException) when (!Orden_DetailsExists(id))
            {
                return NotFound();
            }
        }

    }
}
