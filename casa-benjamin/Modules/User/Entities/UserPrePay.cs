using casa_benjamin.Modules.Shared.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.User.Entities
{
    [Table("user_prepay")]
    public class UserPrePay
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int amount { get; set; }
        public PayType pay_type { get; set; }
        public string comment { get; set; }
        public DateTime prepay_date { get; set; }
        public decimal deposit_credit_card_charge { get;set;}
        public string staff { get; set; }
    }
}