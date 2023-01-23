using casa_benjamin.Managers;
using casa_benjamin.Models; 
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.Kitchen.Services;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{

    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Guests(int? days)
        {
            var model = new List<User>();
            int _days = -1;
            if (days.HasValue)
            {
                _days = days.Value;
            }

            ViewBag.days = _days;
            if (_days == -1)
            {
                model = ReportsManager.Instance.GetStayingGuestsList();
            }
            else
            {
                model = UserManager.Instance.GetUsersByDays(_days);
            }
            return View(model);
        }      

        public ActionResult CheckOutsDue()
        {
            return View(ReportsManager.Instance.GetCheckOutsDue());
        }

        public ActionResult CheckOutsDueCount()
        {
            int count = ReportsManager.Instance.GetCheckOutsDueCount();
            return new JsonResult { JsonRequestBehavior = JsonRequestBehavior.AllowGet, Data = count };
        }

        public ActionResult EndOfShift(int? id)
        {

            DateTime endOfShift;
            Shift lastShift;

            if (id.HasValue)
            {
                Shift existingShift = UserManager.Instance.GetShift(id.Value);
                endOfShift = existingShift.shift_date;
                lastShift = UserManager.Instance.GetShift(id.Value - 1);
                ViewBag.ExistingShift = true;
            }
            else
            {
                endOfShift = DateTime.Now;
                lastShift = UserManager.Instance.GetLastShift();
                ViewBag.ExistingShift = false;
            }

            DateTime? lastShiftDate = lastShift == null ? null : (DateTime?)lastShift.shift_date;
            List<Order> orders = UserManager.Instance.GetShiftOrders(endOfShift, lastShiftDate).OrderBy(x=>x.id).ToList();
            List<OrderItems> orderItems = new List<OrderItems>();
            if(orders.Count > 0)
            {
                orderItems = KitchenManager.Instance.GetOrderItems(orders.First().id, orders.Last().id);
            }

            var cashRegisterEvents = CashRegisterManager.Instance.GetRegisterEvents(lastShiftDate.HasValue ? lastShiftDate.Value : DateTime.Now.AddYears(-1), endOfShift);
            var model = new UIShift
            {
                EndOfShiftDate = endOfShift,
                LastShift = lastShift,
                Orders = orders,
                OrderItems = orderItems,
                TotalCash = (decimal)orders.Where(x => !x.is_canceled && x.pay_type_id == PayType.Cash).Sum(y => y.total),
                TotalCredit = (decimal)orders.Where(x => !x.is_canceled && x.pay_type_id == PayType.Credit).Sum(y => y.total),
                TotalCanceled = (decimal)orders.Where(x => x.is_canceled).Sum(y => y.total),
                Discounts = UserManager.Instance.GetGhostUserDiscounts(lastShiftDate.HasValue ? lastShiftDate.Value: DateTime.Now.AddYears(-1),endOfShift),
                CheckOuts = UserManager.Instance.GetCheckouts(lastShiftDate.HasValue ? lastShiftDate.Value : endOfShift.AddDays(-1), endOfShift)
            };

            model.ExpensesEvents = cashRegisterEvents.Where(x => x.event_type_id == EventType.CashRegisterSubstractFromEmployee
                                                                 || x.event_type_id == EventType.CashRegisterAddExpense).ToList();
            model.TotalExpenses = model.ExpensesEvents.Sum(x => x.event_value);

            model.IncomesEvents = cashRegisterEvents.Where(x => x.event_type_id == EventType.CashRegisterAddFromEmployee
                                                                || x.event_type_id == EventType.CashRegisterAddIncome
                                                                || x.event_type_id == EventType.CashRegisterAddPrePayment
                                                                || x.event_type_id == EventType.CashRegisterUpdatePrePayment
                                                                || x.event_type_id == EventType.CashRegisterRemovePrePayment).ToList();
            model.TotalIncomes = model.IncomesEvents.Sum(x => x.event_value);

            model.TotalCheckoutsCash = (decimal)model.CheckOuts.Sum(x => x.total_cash);
            model.TotalCheckoutsCredit = (decimal)model.CheckOuts.Sum(x => x.total_credit);
            model.Total = model.TotalCash + 
                          model.TotalCredit + 
                          model.TotalCheckoutsCash + 
                          model.TotalCheckoutsCredit +
                          model.TotalIncomes +
                          model.TotalExpenses;

            return View(model);
        }

        [HttpPost]
        public ActionResult CloseShift(UIEndShift uiShift)
        {            
            UserManager.Instance.EndShift(uiShift);
            string message = "alert=Closed shift&alerttype=success";
            return Redirect("/?" + message);
        }

    }
}