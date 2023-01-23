using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.BookKeeping.Enums;
using System;

namespace casa_benjamin.Models
{
    public class UIExpense: Expense
    {
        public ExpenseCategoryType expense_category_type { get; set; }
    }
}