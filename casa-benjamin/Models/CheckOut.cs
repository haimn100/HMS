using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Models
{
    [Table("check_outs")]
    public class CheckOut
    {
        [Key]
        public int id { get; set; }

        public int user_id { get; set; }
        public int bed_id { get; set; }
        public bool user_sex { get; set; }
        public DateTime check_in_date { get; set; }
        public DateTime check_out_date { get; set; }
        public double total_kitchen { get; set; }
        public double total_services { get; set; }
        public double total_accommodation { get; set; }
        public int total_nights { get; set; }
        public double total_cash { get; set; }
        public double total_credit { get; set; }
        public double total_debit { get; set; }
        public double total_discount { get; set; }

        /// <summary>
        /// Total canceled items. these items does not included in the bill subtotal
        /// unlike discounts where they subtracted from the subtotal after
        /// </summary>
        public double total_canceled { get; set; }

        public double total { get; set; }
        public double price_per_night { get; set; }
        public string staff { get; set; }
        public string user_name { get; set; }
        public decimal credit_charge_percentage { get; set; }
        public double cash_deposit { get; set; }
        public double credit_deposit { get; set; }
        public decimal credit_charge_amount { get; set; }

    }
}