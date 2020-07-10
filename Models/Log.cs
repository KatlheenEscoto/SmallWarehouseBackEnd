using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Log
    {
		[Key]
		public int log_id { get; set; }
		public DateTime log_date { get; set; }

		public int orden_details_qty { get; set; }

		public int log_status { get; set; }

		public string item_sku { get; set; }

		[ForeignKey("item_id")]
		public int item_id { get; set; }

		[ForeignKey("usuario_id")]
		public int usuario_id { get; set; }

    }
}
