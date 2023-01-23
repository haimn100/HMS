using Dapper.Contrib.Extensions;

namespace casa_benjamin.Models
{
    [Table("stock")]
    public class StockItem
    {
        [Key]
        public int id { get; set; }
        public int menu_item_id { get; set; }
        public int menu_item_number { get; set; }
        public string menu_item_name { get; set; }
        public string menu_item_category_name { get; set; }
        public int quantity { get; set; }
        public int warning_quantity { get; set; }
    }
}