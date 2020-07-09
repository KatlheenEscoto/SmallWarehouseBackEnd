using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Usuario
    {
        [Key]
        public int usuario_id { get; set; }

        public string usuario_username { get; set; }

        public string usuario_password { get; set; }

        [ForeignKey("rol_id")]
        public int rol_id { get; set; }
    }
}
