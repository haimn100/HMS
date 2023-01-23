using casa_benjamin.Modules.Shared.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_discount")]
    public class UserDiscount
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public string user_name { get; set; }
        public string comment { get; set; }
        public int price { get; set; }
        public DateTime discount_date { get; set; }
        public string staff { get; set; }
        public PayType payment_type_id { get; set; }
        public int order_id { get; set; }
    }
}