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
    public class OrdenController : ControllerBase
    {
        private readonly AppDbContext _context;

        public OrdenController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Orden
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Orden>>> GetOrden()
        {
            return await _context.Orden.ToListAsync();
        }

        // GET: api/Orden/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Orden>> GetOrden(int id)
        {
            var orden = await _context.Orden.FindAsync(id);

            if (orden == null)
            {
                return NotFound();
            }

            return orden;
        }

        private bool OrdenExists(int id)
        {
            return _context.Orden.Any(e => e.orden_id == id);
        }

        /*
         *       
         *
         * 
         * 
         */

        // Custom functions. c:

        // POST: api/orden
        // Agregar Orden.
        [HttpPost]
        [Authorize]
        public async Task<ActionResult<Orden>> PostOrden(Orden orden)
        {
            orden.orden_total_qty = 0; // Todavía no ha comprado nada.
            orden.orden_status = 1; // Orden en proceso de compra.
            _context.Orden.Add(orden);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetOrden", new { id = orden.orden_id }, orden);
        }

        // DELETE: api/orden/5
        [HttpDelete("{id}")]
        [Authorize]
        public async Task<ActionResult<Orden>> DeleteOrden(int id)
        {

            /* Cancelar una orden */
            var orden_delete = await _context.Orden.FindAsync(id);
            if (orden_delete == null)
            {
                return NotFound("La orden no existe.");
            }

            orden_delete.orden_status = 0; // Orden cancelada.
            var orden_details = await _context.Orden_Details.Where( d => d.orden_id  == orden_delete.orden_id && d.orden_details_status == 1).ToListAsync();
            foreach (var detail in orden_details)
            {
                var item = await _context.Item.FindAsync(detail.item_id);
                if(item.item_id == detail.item_id)
                {
                    item.item_qty = item.item_qty + detail.orden_details_qty; // Modificando el stock.
                }
                detail.orden_details_status = 0; // Estado cancelada.
            }

            try
            {
                await _context.SaveChangesAsync();
                return orden_delete;
            }
            catch (DbUpdateConcurrencyException) when (!OrdenExists(id))
            {
                return NotFound();
            }
        }

        // PUT: api/orden/5
        [HttpPut("{id}")]
        [Authorize]
        public async Task<ActionResult<Orden>> PutOrden(int id)
        {

            /* Comprar una orden */
            var orden = await _context.Orden.FindAsync(id);
            if (orden == null)
            {
                return NotFound("La orden no existe.");
            }

            orden.orden_status = 2; // Orden comprada.
            orden.orden_total_qty = 0;
            var orden_details = await _context.Orden_Details.Where(d => d.orden_id == orden.orden_id && d.orden_details_status == 1).ToListAsync();
            foreach (var detail in orden_details)
            {
                detail.orden_details_status = 2; // Estado comprado.
                orden.orden_total_qty = orden.orden_total_qty + detail.orden_details_qty;
            }

            try
            {
                await _context.SaveChangesAsync();
                return orden;
            }
            catch (DbUpdateConcurrencyException) when (!OrdenExists(id))
            {
                return NotFound();
            }
        }



    }
}
