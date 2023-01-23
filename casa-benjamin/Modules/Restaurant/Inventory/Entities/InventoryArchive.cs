using Dapper.Contrib.Extensions;
using System;


namespace casa_benjamin.Modules.Restaurant.Inventory.Entities
{
    [Table("restaurant_inventory_archive")]
    public class InventoryArchive
    {
        [Key]
        public int id { get; set; }
        public string original_id { get; set; }
        public string table_name { get; set; }
        public string record_data_json { get; set; }
        public DateTime created_at { get; set; }
    }
}