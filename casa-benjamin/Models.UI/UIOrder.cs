using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.User.Entities;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIOrder
    {
        public List<OrderItems> items { get; set; }
        public double total { get; set; }
        public PayType pay_type_id { get; set; }
        public List<User> splitUsers { get; set; }
        public int splitCashCount { get; set; }
        public int staffId { get; set; }
        public string staffName { get; set; }
        public int userId { get; set; }
        public int userBed { get; set; }
        public string userName { get; set; }
        public bool isCashUser { get; set; }
        public decimal creditCardChargePercentage { get; set; }
        public MenuCategoryType menu_category_type { get; set; }

        public List<UserDiscount> discounts { get; set; }
    }
}