using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Models
{
    public class UIShift
    {
        public DateTime EndOfShiftDate { get; set; }
        public Shift LastShift { get; set; }
        public List<Order> Orders { get; set; }
        public List<OrderItems> OrderItems { get; set; }
        public List<UserDiscount> Discounts { get; set; }
        public List<CheckOut> CheckOuts{ get; set; }
        public List<CashRegisterEvent> ExpensesEvents{ get; set; }
        public List<CashRegisterEvent> IncomesEvents { get; set; }
        public decimal TotalCheckoutsCash { get; set; }
        public decimal TotalCheckoutsCredit { get; set; }
        public decimal TotalCash { get; set; }
        public decimal TotalCredit { get; set; }
        public decimal TotalCanceled { get; set; }
        public decimal TotalExpenses { get; set; }
        public decimal TotalIncomes { get; set; }
        public decimal Total { get; set; }

    }
}