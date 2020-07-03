using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace SmallWarehouseBackEnd.Models
{
    public class Item
    {
        [Key]
        public int item_id { get; set; }

        [Required(ErrorMessage = "Por favor teclea el campo item_sku.")]
        public string item_sku{ get; set; }
        public string item_description { get; set; }
        public int item_qty { get; set; }
        public int item_status { get; set; }
    }
}
