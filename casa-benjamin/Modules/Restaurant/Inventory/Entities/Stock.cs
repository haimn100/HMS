using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Inventory.Entities
{
    [Table("restaurant_inventory_stock")]
    public class Stock
    {
        [Key]
        public int id { get; set; }
        public int product_id { get; set; }
        public decimal change_in_quantity { get; set; }
        public decimal quantity { get; set; }
        public ProductQuantityChangeOrigin origin { get; set; }
        public string  note { get; set; }
        public int? related_entity { get; set; }
    }
}