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
    public class ItemController : ControllerBase
    {
        private readonly AppDbContext _context;

        public ItemController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Item
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Item>>> GetItem()
        {
            return await _context.Item.ToListAsync();
        }

        // GET: api/Item/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Item>> GetItem(int id)
        {
            var item = await _context.Item.FindAsync(id);

            if (item == null)
            {
                return NotFound();
            }

            return item;
        }

        private bool ItemExists(int id)
        {
            return _context.Item.Any(e => e.item_id == id);
        }

        /*
         *       
         *
         * 
         * 
         */

        // Custom functions. c:

        // GET: api/Item/active
        [HttpGet("active")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Item>>> GetActiveItem()
        {
            return await _context.Item.Where(i => i.item_status == 1).ToListAsync();
        }

        // GET: api/Item/inactive
        [HttpGet("inactive")]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Item>>> GetInactiveItem()
        {
            return await _context.Item.Where(i => i.item_status == 0).ToListAsync();
        }

        // PUT: api/Item/5
        // Modificar Item.
        [HttpPut("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Item>> PutItem(int id, Item item_new)
        {

            var item_old = await _context.Item.FindAsync(id);
            if (item_old == null)
            {
                return NotFound("El item no existe.");
            }

            item_old.item_sku = item_new.item_sku;
            item_old.item_description = item_new.item_description;
            item_old.item_qty = item_new.item_qty;
            item_old.item_status = item_new.item_status;

            try
            {
                await _context.SaveChangesAsync();
                return item_old;
            }
            catch (DbUpdateConcurrencyException) when (!ItemExists(id))
            {
                return NotFound();
            }
        }

        // POST: api/Item
        // Agregar Item.
        [HttpPost]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Item>> PostItem(Item item)
        {
            item.item_status = 1;
            _context.Item.Add(item);
            await _context.SaveChangesAsync();

            return CreatedAtAction("GetItem", new { id = item.item_id }, item);
        }

        // DELETE: api/Item/5
        [HttpDelete("{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Item>> DeleteItem(int id)
        {

            /* Dar de baja un item determinado. item_status = 0 */

            var item_delete = await _context.Item.FindAsync(id);
            if (item_delete == null)
            {
                return NotFound("El item no existe.");
            }
            item_delete.item_status = 0;
            try
            {
                await _context.SaveChangesAsync();
                return item_delete;
            }
            catch (DbUpdateConcurrencyException) when (!ItemExists(id))
            {
                return NotFound();
            }
        }

        // PUT: api/Item/stock/5
        [HttpPut("stock/{id}")]
        [Authorize(Roles = "admin")]
        public async Task<ActionResult<Item>> UpdateItemQty(int id, Item item_new)
        {

            /* Dar de baja un item determinado. item_status = 0 */

            var item_old = await _context.Item.FindAsync(id);
            if (item_old == null)
            {
                return NotFound("El item no existe.");
            }

            item_old.item_qty = item_new.item_qty;

            try
            {
                await _context.SaveChangesAsync();
                return item_old;
            }
            catch (DbUpdateConcurrencyException) when (!ItemExists(id))
            {
                return NotFound();
            }
        }

        /* Búsqueda y paginación */

        // GET: api/Item/index
        /* Documentación base: https://docs.microsoft.com/en-us/aspnet/core/data/ef-mvc/sort-filter-page?view=aspnetcore-3.1&fbclid=IwAR0s4OqoCf9O9SZY4g8UhMWke7iU4X3j6nlkU0XUZjgZBmf6DA8M71_hmA8 */
       
        [HttpGet("index")]
        [Authorize]
        public async Task<ActionResult<PaginatedList>> Index(
            string currentFilter, 
            string searchString,
            int? pageNumber // Contiene nulos. 
            )
        {

            if (searchString != null) // Envio otra cadena para buscar.
            {
                pageNumber = 1; // Reiniciamos busqueda.
            }
            else
            {
                searchString = currentFilter; // Se sigue en la misma busqueda, pero, en otra pagina. 
            }

            // No digita nada el usuario, buscamos todo.
            var items = from i in _context.Item 
                        select i;
            items = items.Where(i => i.item_qty > 0);

            // No es nula la cadena de busqueda, buscamos.
            if(!String.IsNullOrEmpty(searchString))
            {
                items = items.Where(i => i.item_sku.Contains(searchString));
            }



            int pageSize = 3; // Le especifico un valor para mostrar determinada cantidad de item.


            //return await items.AsNoTracking().ToListAsync();
            return await PaginatedList.CreateAsync(items.AsNoTracking(), pageNumber ?? 1, pageSize);
        }

    }
}
