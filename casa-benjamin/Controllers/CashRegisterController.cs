using casa_benjamin.ActionFilters;
using casa_benjamin.Helpers;
using casa_benjamin.Managers;
using casa_benjamin.Models;
using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.BookKeeping.Enums;
using casa_benjamin.Modules.BookKeeping.Services;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor")]
    public class CashRegisterController : Controller
    {
        LedgerService ledger = new LedgerService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);

        // GET: CashRegister
        public ActionResult Index(int? shiftId)
        {
            
            Shift currentShift = null;
            Shift prevShift = null;
            Shift nextShift = null;

            currentShift = shiftId.HasValue ? UserManager.Instance.GetShift(shiftId.Value):
                                           UserManager.Instance.GetLastShift();
            if(currentShift == null)
            {
                currentShift = new Shift
                {
                    id = 0,
                    shift_date = DateTime.Now.AddDays(-7)
                };
            }

            prevShift = UserManager.Instance.GetShift(currentShift.id - 1);
            nextShift = UserManager.Instance.GetShift(currentShift.id + 1);

            ViewBag.PrevShift = prevShift;
            ViewBag.CurrentShift = currentShift;

            List<CashRegisterEvent> model;
            if(shiftId.HasValue)
            {
                if(nextShift != null)
                {
                    model = CashRegisterManager.Instance.GetRegisterEvents(currentShift.shift_date, nextShift.shift_date);
                }
                else
                {
                    model = CashRegisterManager.Instance.GetRegisterEvents(currentShift.shift_date, null);
                }
            }
            else
            {
                model = CashRegisterManager.Instance.GetRegisterEvents(currentShift.shift_date, null);
            }
            ViewBag.CurrentCash = model.Count > 0 ? model.Last().current_register_amount : 0;
            return View(model);
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin")]
        public ActionResult ResetRegister(decimal amount,int staffId, string comment)
        {
            Staff staff = UserManager.Instance.GetStaff(staffId);
            CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
            {
                current_register_amount = amount,
                event_type_id = EventType.CashRegisterReset,
                staff_id = staffId,
                staff_name = staff.name,
                event_value = amount,
                event_date = DateTimeHelper.GetCurrentDateTime(),
                comment = comment
            });

            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult AddToRegister(decimal amount, int staffId, string comment, bool include_in_report = false)
        {
            Staff staff = UserManager.Instance.GetStaff(staffId);
            var lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
            decimal lastAmount = lastEvent == null ? 0 : lastEvent.current_register_amount;

            CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
            {
                current_register_amount = lastAmount + amount,
                event_type_id = EventType.CashRegisterAddFromEmployee,
                staff_id = staffId,
                staff_name = staff.name,
                event_value = amount,
                event_date = DateTimeHelper.GetCurrentDateTime(),
                comment = comment
            });
            return RedirectToAction("Index");
        }

        [HttpPost]
        public ActionResult SubtractRegister(decimal amount, int staffId, string comment)
        {
            Staff staff = UserManager.Instance.GetStaff(staffId);
            var lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
            decimal lastAmount = lastEvent == null ? 0 : lastEvent.current_register_amount;
               
            List<ExpenseCategory> expenseCategories = ReportsManager.Instance.GetExpenseCategories();
            
            CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
            {
                current_register_amount = lastAmount - amount,
                event_type_id = EventType.CashRegisterSubstractFromEmployee,
                staff_id = staffId,
                staff_name = staff.name,
                event_value = amount > 0 ? -amount:amount,
                event_date = DateTimeHelper.GetCurrentDateTime(),
                comment = comment
            });
            return RedirectToAction("Index");
        }

        public List<CashRegisterEvent> GetEvents(int quantity, int offset)
        {
            return CashRegisterManager.Instance.GetRegisterEvents(quantity, offset);
        }
    }
}