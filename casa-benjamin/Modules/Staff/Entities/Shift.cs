using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.Staff.Entities
{
    [Table("shift_end")]
    public class Shift
    {
        [Key]
        public int id { get; set; }

        public DateTime shift_date { get; set; }
        public string shift_employee_name { get; set; }
        public int shift_employee_id { get; set; }
        public decimal shift_total_cash { get; set; }
        public decimal shift_total_credit { get; set; }
        public decimal shift_total_canceled { get; set; }
        public decimal shift_total_checkouts { get; set; }
        public decimal shift_total_kitchen { get; set; }
        public decimal shift_total_services { get; set; }
        public decimal shift_total { get; set; }
        public decimal checkouts_cash { get; set; }
        public decimal checkouts_credit { get; set; }
        public decimal expenses { get; set; }
        public decimal incomes { get; set; }
    }
}