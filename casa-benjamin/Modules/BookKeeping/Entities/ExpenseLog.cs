

using Dapper.Contrib.Extensions;
using System;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("expense_log")]
    public class ExpenseLog
    {
        [Key]
        public int id { get; set; }
        public ExpenseLogActionType action_type { get; set; }
        public int expense_id { get; set; }
        public string expense_name { get; set; }
        public int staff_id { get; set; }
        public string staff_name { get; set; }
        public decimal expense_val { get; set; }

        public DateTime _timestamp { get; set; }
    }
    
    public enum ExpenseLogActionType
    {
        Delete = 1,
        Update = 2
    }
}