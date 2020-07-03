using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Orden
    {
        [Key]
        public int orden_id { get; set; }

        public int orden_total_qty { get; set; }

        public int orden_status { get; set; }

    }
}
