using casa_benjamin.ActionFilters;
using casa_benjamin.Managers;
using casa_benjamin.Models;
using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.BookKeeping.Enums;
using casa_benjamin.Modules.BookKeeping.Services;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace casa_benjamin.Modules.BookKeeping.Controllers
{

    [AuthenticateActionFilter(Roles = "Admin,Editor")]
    public class LedgerController : Controller
    {
        private LedgerService ledger = new LedgerService(
            ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);

        public ActionResult ManageExpenses(DateTime? date)
        {
            date = !date.HasValue ? DateTime.Now : date.Value;
            ViewBag.Date = date.Value; 
            return View("~/Views/Admin/Expenses/AddExpense.cshtml", date);
        }      

        public ActionResult GetExpensesByReportedDate(DateTime? date)
        {
            date = !date.HasValue ? DateTime.Now : date.Value;
            var cats = ledger.AllExpensesCategories();
            var data = ledger.GetExpensesByReportedDate(date).Select(x => {
                return new UIExpense
                {
                    id = x.id,
                    comment = x.comment,
                    expense_category_id = x.expense_category_id,
                    expense_category_type = cats.First(c=> c.id == x.expense_category_id).expense_category_type,
                    expense_date = x.expense_date,
                    expense_val = x.expense_val,
                    report_date = x.report_date,
                    payment_type = x.payment_type
                };
            });
            return new JsonResult { Data = new { data }, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AllExpensesCategories()
        {
            return new JsonResult { Data = ledger.AllExpensesCategories(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult AllExpensesCategoriesTypes()
        {
            var names = Enum.GetNames(typeof(ExpenseCategoryType)).ToList();
            var vals =  Enum.GetValues(typeof(ExpenseCategoryType)).Cast<ExpenseCategoryType>().ToList();
            var res = names.Select((name, i) => new { id = vals[i], name });
            return new JsonResult { Data = res , JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public long AddExpense(Expense item){

            item.expense_category_id = item.expense_category_id == 0 ? (int)ExpenseCategoryType.Other : item.expense_category_id;
            item.expense_date = DateTime.Now;
            item.report_date = new DateTime(item.report_date.Year, item.report_date.Month, 1);
            return new GenericRepository().Insert(item);
        }

        [HttpPost]
        public void UpdateExpense(Expense item)
        {
            var dbItem = ledger.GetExpense(item.id);
            dbItem.comment = item.comment;
            dbItem.expense_category_id = item.expense_category_id;
            dbItem.expense_val = item.expense_val;
            dbItem.payment_type = item.payment_type;

            ledger.UpdateExpense(dbItem);

            var cats = ledger.AllExpensesCategories();
            var category = cats.FirstOrDefault(x => x.id == item.expense_category_id);
            Staff.Entities.Staff staff = (Staff.Entities.Staff)Session["user"];

            ReportsManager.Instance.InsertExepnseLog(new ExpenseLog
            {
                action_type = ExpenseLogActionType.Update,
                expense_id = item.expense_category_id,
                expense_name = category != null ? category.name : "",
                staff_id = staff.id,
                staff_name = staff.name,
                expense_val = (decimal)item.expense_val,
                _timestamp = DateTime.Now
            });
        }

        [HttpPost]
        public void RemoveExpense(int id){

            Expense expense = ledger.GetExpense(id);
            var cats = ledger.AllExpensesCategories();
            ExpenseCategory cat = cats.FirstOrDefault(x => x.id == expense.expense_category_id);
            Staff.Entities.Staff staff = (Staff.Entities.Staff)Session["user"];

            ReportsManager.Instance.InsertExepnseLog(new ExpenseLog
            {
                action_type = ExpenseLogActionType.Delete,
                expense_id = expense.expense_category_id,
                expense_name = cat == null ? "" : cat.name,
                staff_id = staff.id,
                staff_name = staff.name,
                expense_val = (decimal)expense.expense_val,
                _timestamp = DateTime.Now

            });

            ledger.DeleteExpense(expense.id);
        }

    }
}