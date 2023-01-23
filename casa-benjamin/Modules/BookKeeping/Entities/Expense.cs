using casa_benjamin.Modules.BookKeeping.Enums;
using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("expense")]
    public class Expense
    {
        [Key]
        public int id { get; set; }
        public DateTime expense_date { get; set; }
        public int expense_category_id { get; set; }
        public ExpensePaymentType payment_type { get; set; }
        public double expense_val { get; set; }
        public DateTime report_date { get; set; }
        public string comment { get; set; }
    }
}