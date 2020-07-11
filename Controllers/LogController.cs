using System;
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
    public class LogController : ControllerBase
    {
        private readonly AppDbContext _context;

        public LogController(AppDbContext context)
        {
            _context = context;
        }

        // GET: api/Log
        [HttpGet]
        [Authorize]
        public async Task<ActionResult<IEnumerable<Log>>> GetLog()
        {
            return await _context.Log.ToListAsync();
        }

        // GET: api/Log/5
        [HttpGet("{id}")]
        [Authorize]
        public async Task<ActionResult<Log>> GetLog(int id)
        {
            var log = await _context.Log.FindAsync(id);

            if (log == null)
            {
                return NotFound();
            }

            return log;
        }

        
        private bool LogExists(int id)
        {
            return _context.Log.Any(e => e.log_id == id);
        }
    }
}
