using casa_benjamin.Modules.BookKeeping.Entities;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIExpenses
    {
        public List<Expense> Expenses { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalRevenue { get; set; }
        public List<decimal> ExpensesGraph { get; set; }
    }
}