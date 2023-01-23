using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.Restaurant.Inventory.Entities
{
    [Table("restaurant_inventory_supplier")]
    public class Supplier
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public string email { get; set; }
    }
}