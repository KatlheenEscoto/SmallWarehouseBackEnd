using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
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

        public string usuario_email { get; set; }
    }
}
