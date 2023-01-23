using casa_benjamin.Modules.BookKeeping.Enums;
using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("expense_category")]
    public class ExpenseCategory
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public ExpenseCategoryType expense_category_type { get; set; }
    }
}