using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace casa_benjamin.Modules.Restaurant.Menu.Values
{
    public class UpdateMenuItemDTO
    {
        public int id { get; set; }
        public string name { get; set; }
        public int number { get; set; }
        public int price { get; set; }
        public bool is_active { get; set; }
        public decimal product_weight { get; set; }
        public int? product_id { get; set; }
    }
}