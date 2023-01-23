using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Models
{
    [Table("stock_history")]
    public class StockHistoryItem
    {
        [Key]
        public int id { get; set; }
        public int menu_item_id { get; set; }
        public int menu_item_number { get; set; }
        public string menu_item_name { get; set; }
        public string menu_item_category_name { get; set; }
        public int quantity { get; set; }
        public int total { get; set; }
        public DateTime timestamp { get; set; }
    }
}