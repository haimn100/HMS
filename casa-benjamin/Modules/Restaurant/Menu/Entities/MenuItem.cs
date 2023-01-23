using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Menu.Entities
{
    [Table("menu_item")]
    public class MenuItem
    {
        [Key]
        public int id { get; set; }

        public string name { get; set; }
        public int cat_id { get; set; }
        public int number { get; set; }
        public int price { get; set; }
        public bool is_active { get; set; }
        public bool is_deleted { get; set; }
        public MenuCategoryType menu_category_type { get; set; }

        /// <summary>
        /// By default the consumption is calculated per day per visitor
        /// </summary>
        public decimal consumption { get; set; }
        public decimal product_weight { get; set; }
        public int? product_id { get; set; }
    }
}