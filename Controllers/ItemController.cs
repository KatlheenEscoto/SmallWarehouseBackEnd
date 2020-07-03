using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using SmallWarehouseBackEnd.Contexts;
using SmallWarehouseBackEnd.Models;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace SmallWarehouseBackEnd.Controllers
{
    [Route("api/[controller]")]
    public class ItemController : Controller
    {
        private readonly AppDbContext context;

        public ItemController(AppDbContext context)
        {
            this.context = context;
        }

        // GET: api/<controller>
        [HttpGet]
        public IEnumerable<Item> Get()
        {
            return context.Item.ToList();
        }

        // GET api/<controller>/5
        [HttpGet("{id}")]
        public Item Get(int id)
        {
            var item = context.Item.FirstOrDefault(i => i.item_id == id);
            return item;
        }

        // POST api/<controller>
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }

        // PUT api/<controller>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }

        // DELETE api/<controller>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
