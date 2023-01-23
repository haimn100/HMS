
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using Dapper.Contrib.Extensions;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Modules.Restaurant.Order.Entities
{
    [Table("order_items")]
    public class OrderItems
    {
        [Key]
        public int id { get; set; }

        public int order_id { get; set; }
        public int menu_item_id { get; set; }
        public string menu_item_name { get; set; }
        public string menu_item_ingredients { get; set; }

        [Computed]
        public List<int> menu_item_ingredients_ids { get; set; }
        public int menu_category_id { get; set; }
        public string menu_category_name { get; set; }
        public MenuCategoryType menu_category_type { get; set; }    
        public string comment { get; set; }
        public double split_total { get; set; }
        public double total { get; set; }
        public int user_id { get; set; }
        public DateTime order_date { get; set; }
    }
}