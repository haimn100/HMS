using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using casa_benjamin.Models;
using casa_benjamin.Managers;
using casa_benjamin.ActionFilters;
using casa_benjamin.Finance;
using casa_benjamin.Internalization;
using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using casa_benjamin.Modules.BookKeeping.Enums;
using casa_benjamin.Modules.User.Services;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.BookKeeping.Services;
using System.Configuration;

namespace casa_benjamin.Controllers
{

    [AuthenticateActionFilter(Roles = "Admin")]
    public class ReportsController : Controller
    {
        public ActionResult DailyDashboard(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;           
            return View("~/Views/Admin/Reports/PeriodDashboard.cshtml");
        }

        public ActionResult MonthlyDashboard(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;
            return View("~/Views/Admin/Reports/PeriodDashboard.cshtml");
        }

        public ActionResult IncomeStatement(ReportDateQuery dateQuery)
        {
            var model = new FinanceAdvisor().GetDailyIncomeStatement(dateQuery.From);
            ViewBag.DateQuery = dateQuery;
            return View("~/Views/Admin/Reports/Finance/DailyIncomeStatement.cshtml",model);
        }

        
        public ActionResult MonthlyIncomeStatement(ReportDateQuery dateQuery)
        {            
            var model = new FinanceAdvisor().MonthlyIncomeStatement(dateQuery.From);
            ViewBag.DateQuery = dateQuery;
            return View("~/Views/Admin/Reports/Finance/MonthlyIncomeStatement.cshtml",model);
        }
        public ActionResult MonthlyIncomeStatement2(ReportDateQuery dateQuery)
        {
            DateTime date = dateQuery.FromOrDefault();
            var model = new FinanceAdvisor().MonthlyIncomeStatement(dateQuery.From);                     
            ViewBag.DateQuery = dateQuery;
            return View("~/Views/Admin/Reports/Finance/MonthlyIncomeStatement2.cshtml", model);
        }

        public ActionResult CheckOuts(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            Dictionary<string, object> model = new Dictionary<string, object>();
            model = ReportsManager.Instance.GetCheckOutsReport(dateQuery);
            return View("~/Views/Admin/Reports/CheckOutsReport.cshtml", model);
        }

        public ActionResult CheckoutsTable(PagedTableRequest req)
        {
            req.table = "checkouts";
            return new JsonResult { Data = new GenericRepository().GetTable<CheckOut>(req), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult StaffEvents(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;

            var model = ReportsManager.Instance.GetStaffEventsReport(dateQuery);
            return View("~/Views/Admin/Reports/StaffEvent.cshtml", model);
        }

        public ActionResult Shifts(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;

            List<Shift> model = new List<Shift>();
            model = ReportsManager.Instance.GetShifts(dateQuery);
            return View("~/Views/Admin/Reports/Shifts.cshtml", model);
        }

        public ActionResult CheckIns(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;            
            var model = ReportsManager.Instance.GetCheckinsReport(dateQuery);
            return View("~/Views/Admin/Reports/Checkins.cshtml", model);
        }

        public ActionResult Stock(ReportDateQuery dateQuery,int itemId)
        {
            ViewBag.DateQuery = dateQuery;
            ViewBag.ItemID = itemId;
            var model = ReportsManager.Instance.GetStockReport(dateQuery, itemId);
            return View("~/Views/Admin/Reports/Stock.cshtml", model);
        }

        public ActionResult MonthlyCheckIns(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.GetCheckinsReport(dateQuery);
            return View("~/Views/Admin/Reports/Checkins.cshtml", model);
        }

        public ActionResult MonthlyCheckOuts(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;

            Dictionary<string, object> model = new Dictionary<string, object>();
            model = ReportsManager.Instance.GetCheckOutsReport(dateQuery);
            return View("~/Views/Admin/Reports/CheckOutsMonthlyReport.cshtml", model);
        }

        public ActionResult MonthlyShifts(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;

            if(!dateQuery.From.HasValue && !dateQuery.To.HasValue)
            {
                dateQuery.From = DateTime.Now;
                dateQuery.To = DateTime.Now;
            }

            List<Shift> model = new List<Shift>();
            model = ReportsManager.Instance.GetShifts(dateQuery);
            return View("~/Views/Admin/Reports/ShiftsMonthly.cshtml", model);
        }

        public ActionResult MonthlyDiscount(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            dateQuery.Monthly = true;
            var model = ReportsManager.Instance.UserDiscounts(dateQuery);

            return View("~/Views/Admin/Reports/Discounts.cshtml", model);
        }
        public ActionResult MonthlyDeposit(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            dateQuery.Monthly = true;
            DateTime _from = dateQuery.FromOrDefault();
            _from = new DateTime(_from.Year, _from.Month, 1);
            DateTime _to = dateQuery.ToOrDefault();
            if(_to.Month == _from.Month)
            {
                _to = _from.AddMonths(1).AddDays(-1);
            }
            else
            {
                _to = new DateTime(_to.Year, _to.Month, 1);
            }

            var model = new FinanceAdvisor().GetDailyDeposits(_from, _to);

            return View("~/Views/Admin/Reports/Deposits.cshtml", model);
        }

        public ActionResult DailyDiscount(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.UserDiscounts(dateQuery);

            return View("~/Views/Admin/Reports/DailyDiscounts.cshtml", model);
        }

        public ActionResult DailyOrders(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.GetOrders(dateQuery,MenuCategoryType.Kitchen);
            return View("~/Views/Admin/Reports/Orders.cshtml", model);
        }

        public ActionResult OrdersByStaff(ReportDateQuery dateQuery,int? staff_id)
        {
            if(dateQuery.From == null)
            {
                dateQuery.From = new DateTime(DateTime.Now.Year,DateTime.Now.Month,DateTime.Now.Day);
                dateQuery.To = dateQuery.From.Value.AddDays(1);
            }

            Staff staff = staff_id == null ? CacheManager.Instance.Staff.First(): CacheManager.Instance.Staff.First(x => x.id == staff_id);
            ViewBag.DateQuery = dateQuery;
            ViewBag.Staff = staff;
            var model = ReportsManager.Instance.GetOrdersByStaff(dateQuery, staff.id);
            return View("~/Views/Admin/Reports/OrdersByStaff.cshtml", model);
        }

        public ActionResult DailyServiceOrders(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.GetOrders(dateQuery, MenuCategoryType.Service);
            return View("~/Views/Admin/Reports/Orders.cshtml", model);
        }

        public ActionResult MonthlyOrders(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.GetOrders(dateQuery, MenuCategoryType.Kitchen);
            return View("~/Views/Admin/Reports/Orders.cshtml", model);
        }

        public ActionResult MonthlyServiceOrders(ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;
            var model = ReportsManager.Instance.GetOrders(dateQuery, MenuCategoryType.Service);
            return View("~/Views/Admin/Reports/Orders.cshtml", model);
        }

        [AuthenticateActionFilter(Roles = "Admin,KitchenManager,Editor")]
        public ActionResult DailyMenuItemDetails(ReportDateQuery dateQuery, int menuItemId)
        {
            ViewBag.DateQuery = dateQuery;
            ViewBag.MenuItemID = menuItemId;
            List<OrderItems> orderItems = ReportsManager.Instance.GetOrdersItemsByMenuItemId(dateQuery,menuItemId);
            List<Order> orders = ReportsManager.Instance.GetOrdersByOrderId(orderItems.Select(x => x.order_id).ToList());

            List<OrderRow> model = new List<OrderRow>();
            foreach (var oi in orderItems)
            {
                model.Add(new OrderRow
                {
                    OrderItems = new List<OrderItems> { oi },
                    Order = orders.First(x => x.id == oi.order_id)
                });
            }

            return View("~/Views/Admin/Reports/MenuItemDetails.cshtml",model);
        }

        public ActionResult MonthlyMenuItemDetails(ReportDateQuery dateQuery,int menuItemId)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;
            List<OrderItems> orderItems = ReportsManager.Instance.GetOrdersItemsByMenuItemId(dateQuery, menuItemId);
            List<Order> orders = ReportsManager.Instance.GetOrdersByOrderId(orderItems.Select(x => x.order_id).ToList());

            List<OrderRow> model = new List<OrderRow>();
            foreach (var oi in orderItems)
            {
                model.Add(new OrderRow
                {
                    OrderItems = new List<OrderItems> { oi },
                    Order = orders.First(x => x.id == oi.order_id)
                });
            }

            return View("~/Views/Admin/Reports/MenuItemDetails.cshtml", model);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult AddExpenses(DateTime? date)
        {
            date = !date.HasValue ? DateTime.Now : date.Value;
            ViewBag.Date = date.Value;
            return View("~/Views/Admin/Expenses/AddExpenses.cshtml",date);
        }       

        //public PagedTableResponse<Expense> GetExpensesTable(PagedTableRequest req, DateTime? date)
        //{
        //    date = !date.HasValue ? DateTime.Now : date.Value;
        //    return ledger.GetExpensesTable(new PagedTableRequest
        //    {
        //        predicate = string.Format("month(report_date) = {0} and year(report_date) = {1}", date.Value.Month, date.Value.Year),
        //        dateField = "expense_date",
        //        sortBy = "id",
        //        sortDesc = true
        //    });
        //}

        //public List<ExpenseCategory> GetExpenseCategories()
        //{
        //    return ReportsManager.Instance.GetExpenseCategories();
        //}

        //[AuthenticateActionFilter(Roles = "Admin,Editor")]
        //[HttpPost]
        //public ActionResult AddExpense(int category,string comment, int expense,string returnurl,int reportmonth,int reportyear)
        //{
        //    new GenericRepository().Insert(new Expense
        //    {
        //        expense_category_id = category == 0 ? (int)ExpenseCategoryType.Other: category,
        //        expense_val = expense,
        //        expense_date = DateTime.Now,
        //        report_date = new DateTime(reportyear,reportmonth,1),
        //        comment = comment
        //    });

        //    return Redirect(returnurl);
        //}

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void DeleteExpense(int id)
        {
            Expense e = ReportsManager.Instance.GetExepnse(id);

            new GenericRepository().Delete(e);

            var cats = ReportsManager.Instance.GetExpenseCategories();
            ExpenseCategory cat = cats.FirstOrDefault(x => x.id == e.expense_category_id);
            Staff staff = (Staff)Session["user"];

            ReportsManager.Instance.InsertExepnseLog(new ExpenseLog
            {
                action_type = ExpenseLogActionType.Delete,
                expense_id = e.expense_category_id,
                expense_name = cat == null ? "" : cat.name,
                staff_id = staff.id,
                staff_name = staff.name,
                expense_val = (decimal)e.expense_val
            });
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult AddIncomes(DateTime? date)
        {
            date = !date.HasValue ? DateTime.Now : date.Value;
            var incomesCategories = ReportsManager.Instance.GetIncomeCategories();
            ViewBag.Date = date.Value;

            var result = new GenericRepository().GetTable<Income>(new PagedTableRequest
            {
                predicate = string.Format("month(report_date) = {0} and year(report_date) = {1}", date.Value.Month, date.Value.Year),
                dateField = "date",
                sortBy = "id",
                sortDesc = true,
                table = "income"
            });

            ViewBag.LogTable = new GenericRepository().Get<IncomeLog>(string.Format("select * from income_log where month(_timestamp) = {0} and year(_timestamp) = {1}", date.Value.Month, date.Value.Year)).ToList();

            var model = new List<UIIncome>();
            if (result.data.Count > 0)
            {
                model = result.data.Select(x => new UIIncome
                {   
                    id = x.id,
                    category_id = x.category_id,
                    date = x.date,
                    val = x.val,
                    name = GetIncomeCategoryOrDefault(x.category_id, incomesCategories).name,
                    type = GetIncomeCategoryOrDefault(x.category_id, incomesCategories).income_category_type.ToString(),
                    comment = x.comment
                }).ToList();
            }
            return View("~/Views/Admin/Expenses/AddIncomes.cshtml", model);
        }

        private IncomeCategory GetIncomeCategoryOrDefault(int id, List<IncomeCategory> incomesCategories)
        {
            var icat = incomesCategories.FirstOrDefault(y => y.id == id);
            return icat != null ? icat : new IncomeCategory { income_category_type = IncomeCategoryType.Other, name = "Unknown" };

        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        [HttpPost]
        public ActionResult AddIncome(int category, string comment, int income, string returnurl, int reportmonth, int reportyear)
        {
            new GenericRepository().Insert(new Income
            {
                category_id = category,
                val = income,
                date = DateTime.Now,
                report_date = new DateTime(reportyear, reportmonth, 1),
                comment = comment
            });

            return Redirect(returnurl);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void DeleteIncome(int id)
        {
            Income income = ReportsManager.Instance.GetIncome(id);
            new GenericRepository().Delete(income);

            var cats = ReportsManager.Instance.GetIncomeCategories();
            IncomeCategory cat = cats.FirstOrDefault(x => x.id == income.category_id);
            Staff staff = (Staff)Session["user"];

            ReportsManager.Instance.InsertIncomeLog(new IncomeLog
            {
                action_type = IncomeLogActionType.Delete,
                income_id = income.category_id,
                income_name = cat == null ? "" : cat.name,
                staff_id = staff.id,
                staff_name = staff.name,
                income_val = income.val
            });
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult ExpensesTable(ReportDateQuery dateQuery)
        {
            DateTime from = dateQuery.FromOrDefault();
            from = new DateTime(from.Year, from.Month, 1);

            DateTime? _to = dateQuery.To;
            DateTime to = _to.HasValue ? new DateTime(_to.Value.Year, _to.Value.Month, 1) :
                                         from.AddMonths(1);

            dateQuery.From = from;
            dateQuery.To = to;


            ViewBag.DateQuery = dateQuery;
            ViewBag.Title = I18n.T("Expenses Report");
            ViewBag.DataUrl = "/reports/expensestable";
            ViewBag.DetailTableUrl = "/reports/expensedetailstable";

            var model = ReportsManager.Instance.GetExepnsesTable(from, to);
            return View("~/Views/Admin/Expenses/ExpensesTable.cshtml", model);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult IncomeTable(ReportDateQuery dateQuery)
        {
            DateTime from = dateQuery.FromOrDefault();
            from = new DateTime(from.Year, from.Month,1);

            DateTime? _to = dateQuery.To;
            DateTime to = _to.HasValue ? new DateTime(_to.Value.Year, _to.Value.Month, 1) :
                                         from.AddMonths(1);

            dateQuery.From = from;
            dateQuery.To = to;

            ViewBag.DateQuery = dateQuery;
            ViewBag.Title = I18n.T("Income Report");
            ViewBag.DataUrl = "/reports/incometable";
            ViewBag.DetailTableUrl = "/reports/incomedetailstable";

            var model = ReportsManager.Instance.GetIncomesTable(from,to);
            return View("~/Views/Admin/Expenses/IncomesTable.cshtml", model);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult ExpenseDetailsTable(ReportDateQuery dateQuery, int id = -1)
        {
            DateTime from = dateQuery.FromOrDefault();
            from = new DateTime(from.Year, from.Month, 1);

            DateTime? _to = dateQuery.To;
            DateTime to = _to.HasValue ? new DateTime(_to.Value.Year, _to.Value.Month,1) :
                                         from.AddMonths(1);      

            ViewBag.DateQuery = dateQuery;
            ViewBag.Title = I18n.T("Expense Details Report");
            ViewBag.EntityId = id;
            ViewBag.DataUrl = "/reports/expensedetailstable";
            var model = ReportsManager.Instance.GetExepnsesDetailsTable(from,to, id);
            return View("~/Views/Admin/Expenses/ExpenseDetails.cshtml", model);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult IncomeDetailsTable(ReportDateQuery dateQuery, int id = -1)
        {
            DateTime from = dateQuery.FromOrDefault();
            from = new DateTime(from.Year, from.Month, 1);

            DateTime? _to = dateQuery.To;
            DateTime to = _to.HasValue ? new DateTime(_to.Value.Year, _to.Value.Month, 1) :
                                         from.AddMonths(1);

            dateQuery.From = from;
            dateQuery.To = to;

            ViewBag.DateQuery = dateQuery;
            ViewBag.Title = I18n.T("Income Details Report");
            ViewBag.EntityId = id;
            ViewBag.DataUrl = "/reports/incomedetailstable";
            var model = ReportsManager.Instance.GetIncomesDetailsTable(from, to, id);
            return View("~/Views/Admin/Expenses/IncomeDetails.cshtml", model);
        }

        public ActionResult Residents()
        {
            List<Resident> residents = UserManager.Instance.GetResidents();
            List<User> model = residents.Count == 0 ? new List<User>() : UserManager.Instance.GetUsers(residents.Select(x => x.user_id).ToList());
            return View("~/Views/Admin/Reports/Residents.cshtml",model);
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult ExpenseAdd(Expense expense)
        {
            ExpenseCategory expenseCategory = ReportsManager.Instance.GetExpenseCategory(expense.expense_category_id);

            ReportsManager.Instance.AddExepnse(new Expense
            {
                expense_date = expense.expense_date,
                expense_category_id = expense.expense_category_id,
                expense_val = expense.expense_val
            });

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult ExpenseCategory()
        {
            List<ExpenseCategory> model = ReportsManager.Instance.GetExpenseCategories(true);
            return View("~/Views/Admin/Expenses/Categories.cshtml",model);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult IncomeCategory()
        {
            List<IncomeCategory> model = ReportsManager.Instance.GetIncomeCategories(true);
            return View("~/Views/Admin/Expenses/IncomeCategories.cshtml", model);
        }

        public ActionResult GuestCount()
        {
            return View("~/Views/Admin/Reports/GuestCount.cshtml");
        }


        public ActionResult GuestList(DateTime date)
        {
            var model = ReportsManager.Instance.GetGuestsList(date);
            ViewBag.Date = date;

            return View("~/Views/Admin/Reports/GuestList.cshtml",model);
        }

        public JsonResult GetGuestCount(ReportDateQuery dateQuery)
        {
            var guestCount = ReportsManager.Instance.GetGuestsCount(dateQuery);

            var result = guestCount.Select(x => new {
                title = x.count,
                allDay = true,
                start = x.guestcount_date.ToString("yyyy-MM-dd")
            });

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void UpdateExpenseCategory(ExpenseCategory ec)
        {
            new GenericRepository().Update(ec);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void DeleteExpenseCategory(int id)
        {
            new GenericRepository().ExecuteScalar("update expense_category set is_active = 0 where id = " + id);

            var cats = ReportsManager.Instance.GetExpenseCategories();
            ExpenseCategory cat = cats.FirstOrDefault(x => x.id == id);
            Staff staff = (Staff)Session["user"];

            ReportsManager.Instance.InsertExepnseLog(new ExpenseLog
            {
                action_type = ExpenseLogActionType.Delete,
                expense_id = id,
                expense_name = cat == null ? "" : cat.name,
                staff_id = staff.id,
                staff_name = staff.name
            });
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public long InsertExpenseCategory(string name)
        {
            return new GenericRepository().Insert(new ExpenseCategory { name = name });
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public long InsertIncomeCategory(string name)
        {
            return new GenericRepository().Insert(new IncomeCategory { name = name });
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void UpdateIncomeCategory(IncomeCategory ec)
        {
            new GenericRepository().Update(ec);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public void DeleteIncomeCategory(int id)
        {
            new GenericRepository().ExecuteScalar("update income_category set is_active = 0 where id = " + id);
        }

        public JsonResult GetPeriodData (ReportDateQuery dateQuery)
        {
            return Json(ReportsManager.Instance.GetPeriodData(dateQuery), JsonRequestBehavior.AllowGet);
        }

        [AuthenticateActionFilter(Roles = "Admin,KitchenManager,Editor")]
        public JsonResult GetOrderItemsCountList(MenuCategoryType catType,ReportDateQuery dateQuery)
        {
            var items = ReportsManager.Instance.GetOrderItemsCountList(catType, dateQuery);
            return Json(items, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCheckOuts(ReportDateQuery dateQuery)
        {
            return Json(ReportsManager.Instance.GetCheckOutsReport(dateQuery), JsonRequestBehavior.AllowGet);
        }
    
        public void ExpenseUpdateVal(int id, double val)
        {
            Expense ex = ReportsManager.Instance.GetExepnse(id);
            ex.expense_val += val;
            ReportsManager.Instance.UpdateExepnse(ex);
        }

        [AuthenticateActionFilter(Roles = "Admin,KitchenManager,Editor")]
        public ActionResult MonthlyExpense(int expenseCategoryId, ReportDateQuery dateQuery)
        {
            dateQuery.Monthly = true;
            ViewBag.DateQuery = dateQuery;          
            ViewBag.ExpenseCategoryType = expenseCategoryId;          
            return View("~/Views/Admin/Reports/MonthlyExpense.cshtml");
        }


        //public void DummyUsers()
        //{
        //    GenericRepository rep = new GenericRepository();

        //    DateTime startTime = DateTime.Now;
        //    List<DateTime> dates = new List<DateTime>();


        //    Random rand = new Random();
        //    dates.Add(startTime);
        //    for (int i = 0; i < 90; i++)
        //    {
        //        DateTime d = startTime.AddDays(-1);
        //        dates.Add(d);
        //        startTime = startTime.AddDays(-1);

        //        Shift shift = new Shift
        //        {
        //            shift_date = d,
        //            shift_employee_id = 3,
        //            shift_employee_name = "yaniv",
        //            shift_total_canceled = rand.Next(0, 10000),
        //            shift_total_cash = rand.Next(1000, 30000),
        //            shift_total_credit = rand.Next(1000, 30000)
        //        };
        //        shift.shift_total = shift.shift_total_cash + shift.shift_total_credit;
        //        rep.Insert(shift);
        //    }



        //    //for (int i = 0; i < 300; i++)
        //    //{
        //    //    User user = new Models.User
        //    //    {
        //    //        bed_id = rand.Next(1, 500),
        //    //        checked_in_by = "TEST",
        //    //        cidate = dates[rand.Next(0, 89)],
        //    //        is_checked_out = false,
        //    //        name = rand.Next(0, 100000).ToString(),
        //    //        passport = rand.Next(10000, 50000),
        //    //        nationality = "Israel",
        //    //        pic = "111111.png",
        //    //        sex = (i % 4 == 0) ? false : true
        //    //    };

        //    //    if ((DateTime.Now - user.cidate).Days > 10)
        //    //    {
        //    //        user.is_checked_out = true;
        //    //        user.codate = user.cidate.AddDays(rand.Next(1, 9));
        //    //    }

        //    //    rep.Insert(user);

        //    //    if (user.codate.HasValue)
        //    //    {
        //    //        CheckOut co = new CheckOut
        //    //        {
        //    //            bed_id = user.bed_id,
        //    //            check_in_date = user.cidate,
        //    //            check_out_date = user.codate.Value,
        //    //            staff = "yaniv",
        //    //            price_per_night = rand.Next(400, 800),
        //    //            total_accommodation = rand.Next(400, 20000),
        //    //            total_cash = rand.Next(0, 1000),
        //    //            total_credit = rand.Next(0, 1000),
        //    //            total_debit = rand.Next(0, 20000),
        //    //            total_kitchen = rand.Next(0, 10000),
        //    //            total_nights = rand.Next(0, 6),
        //    //            user_id = user.id,
        //    //            user_sex = user.sex
        //    //        };
        //    //        co.total = co.total_accommodation + co.total_kitchen;

        //    //        rep.Insert(co);
        //    //    }
        //    //}
        //}

        //public void DummyOrders()
        //{
        //    GenericRepository rep = new GenericRepository();
        //    KitchenRepository kitchRep = new KitchenRepository();

        //    List<User> users = rep.Get<User>("select * from user where is_hidden is null").ToList();

        //    Random rand = new Random();
        //    int index = 1;
        //    foreach (var user in users)
        //    {
        //        int randomOrders = rand.Next(1, 10);

        //        for (int i = 0; i < randomOrders; i++)
        //        {
        //            Order order = new Order
        //            {
        //                is_canceled = (index % 30) == 0,
        //                menu_category_type = MenuCategoryType.Kitchen,
        //                pay_type_id = (PayType)rand.Next(1, 3),
        //                order_date = user.cidate.AddDays(rand.Next(0, 8)),
        //                user_bed = user.bed_id,
        //                user_id = user.id,
        //                user_name = user.name,
        //                staff_id = 3
        //            };

        //            List<OrderItems> items = new List<OrderItems>();
        //            for (int x = 0; x < rand.Next(1,4); x++)
        //            {
        //                UIMenuCategory mc= CacheManager.Instance.MenuCategories[rand.Next(0, CacheManager.Instance.MenuCategories.Count-1)];
        //                MenuItem mi = mc.menuItems[rand.Next(0, mc.menuItems.Count - 1)].menuItem;
        //                items.Add(new OrderItems
        //                {
        //                    menu_category_id = mc.category.id,
        //                    menu_category_name = mc.category.name,
        //                    menu_category_type = mc.category.menu_category_type,
        //                    menu_item_id = mi.id,
        //                    menu_item_name = mi.name,
        //                    order_date = order.order_date,
        //                    user_id = user.id,
        //                    total = mi.price
        //                });
        //            }

        //            order.total = items.Sum(x => x.total);

        //            kitchRep.InsertOrder(order, items);
        //        }

        //    }
        //}
    }
}