using Microsoft.EntityFrameworkCore;
using SmallWarehouseBackEnd.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Contexts
{
    public class AppDbContext: DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options): base(options)
        {

        }

        public DbSet<Item> Item { get; set; }

        public DbSet<Orden> Orden { get; set; }

        public DbSet<SmallWarehouseBackEnd.Models.Orden_Details> Orden_Details { get; set; }
    }
}
