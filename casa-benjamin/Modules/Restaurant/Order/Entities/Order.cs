
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Shared.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.Restaurant.Order.Entities
{
    [Table("menu_order")]
    public class Order
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int user_bed { get; set; }
        public string user_name { get; set; }
        public DateTime order_date { get; set; }
       
        public PayType pay_type_id { get; set; }
        public bool is_canceled { get; set; }
        public string staff_name { get; set; }
        public int staff_id { get; set; }
        public double total { get; set; }
        public int split_count { get; set; }
        public double split_total { get; set; }
        public int splited_by { get; set; }
        public int splited_order_id { get; set; }
        public int canceled_by_staff_id { get; set; }
        public MenuCategoryType menu_category_type { get; set; }
        public string comment { get; set; }
        public decimal credit_charge_percentage { get; set; }
        public decimal credit_charge { get; set; }
        public int discount { get; set; }

        public Order ShallowCopy()
        {
            return (Order)MemberwiseClone();
        }
    }

}