using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace casa_benjamin.Modules.BookKeeping.Services
{
    public class LedgerService
    {
        private GenericRepository repository;
        private const string INCOME_TABLE = "income";
        private const string EXPENSES_TABLE = "expense";
        private const string EXPENSES_CAT_TABLE = "expense_category";

        public LedgerService(string dbConnectionString)
        {
            repository = new GenericRepository();
        }

        #region Expenses

        public void DeleteExpense(int id)
        {
            repository.Delete(new Expense { id = id });
        }

        public void UpdateExpense(Expense item)
        {
            repository.Update(item);
        }

        public Expense GetExpense(int id)
        {
           return repository.Get<Expense>($"select * from {EXPENSES_TABLE} where id = {id}").FirstOrDefault();
        }

        public List<Expense> GetExpensesByReportedDate(DateTime? reportDate)
        {
            var date = !reportDate.HasValue ? DateTime.Now : reportDate.Value;
            return  repository.Get<Expense>($"select * from {EXPENSES_TABLE} where month(report_date) = {date.Month} and year(report_date) = {reportDate.Value.Year}");
        }

        public List<ExpenseCategory> AllExpensesCategories(bool onlyActive = false)
        {
            return onlyActive ? repository.Get<ExpenseCategory>($"select * from {EXPENSES_CAT_TABLE} where is_active = 1 or is_active is null").ToList()
                : repository.GetAll<ExpenseCategory>();
        }

        #endregion

        #region Income
        public long AddIncome(Income item)
        {
            return repository.Insert(item);
        }
        #endregion

    }
}