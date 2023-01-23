using casa_benjamin.Modules.BookKeeping.Enums;
using Dapper.Contrib.Extensions;

namespace casa_benjamin.Modules.BookKeeping.Entities
{
    [Table("income_category")]
    public class IncomeCategory
    {
        [Key]
        public int id { get; set; }
        public string name { get; set; }
        public IncomeCategoryType income_category_type { get; set; }
    }
}