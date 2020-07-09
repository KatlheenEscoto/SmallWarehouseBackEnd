using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Rol
    {
        [Key]
        public int rol_id { get; set; }

        public string rol_name { get; set; }
    }
}
