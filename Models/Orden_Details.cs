using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Orden_Details
    {
        [Key]
        public int orden_details_id { get; set; }

        public int orden_details_qty { get; set; }

        public int orden_details_status { get; set; }

        [ForeignKey("item_id")]
        public int item_id { get; set; }

        [ForeignKey("orden_id")]
        public int orden_id { get; set; }

    }
}
