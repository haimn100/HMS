

using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("income_log")]
    public class IncomeLog
    {
        [Key]
        public int id { get; set; }
        public IncomeLogActionType action_type { get; set; }
        public int income_id { get; set; }
        public string income_name { get; set; }
        public int staff_id { get; set; }
        public string staff_name { get; set; }
        public decimal income_val { get; set; }

    }

    public enum IncomeLogActionType
    {
        Delete = 1
    }
}