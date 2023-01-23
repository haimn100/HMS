using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.User.Entities;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Finance.Models
{
    /// <summary>
    /// Holds periodic income statment
    /// </summary>
    public class DailyIncomeStatment
    {

        public DateTime Date { get; set; }
        public decimal TotalRevenue { get; set; }
        public CheckOuts CheckOutsReport { get; set; }
   
        public CashOrders CashOrdersReport { get; set; }
        public CreditOrders CreditOrdersReport { get; set; }
        public Expenses ExpensesReport{ get; set; }
        public Incomes IncomesReport { get; set; }
        public Deposits DepositsReport { get; set; }

        public class CheckOuts
        {
            public decimal Total { get; set; }
            public decimal TotalPaid { get; set; }
            public decimal TotalPeople { get; set; }
            public decimal TotalServices { get; set; }
            public decimal TotalKitchen { get; set; }
            public decimal TotalAccomodation { get; set; }
            public decimal TotalPaidInCash { get; set; }
            public decimal TotalPaidInCredit { get; set; }
            public decimal TotalDiscount { get; set; }
            public decimal TotalCanceled { get; set; }
            public decimal TotalCashDeposit { get; set; }
            public decimal TotalCreditDeposit { get; set; }
            public decimal TotalCreditChargeAmount { get; set; }

        }      

        public class CashOrders
        {
            public decimal Kitchen { get; set; }
            public decimal Services { get; set; }
            public decimal Discount { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Total { get; set; }
        }
      
        public class CreditOrders
        {
            public decimal Kitchen { get; set; }
            public decimal Services { get; set; }
            public decimal Discount { get; set; }
            public decimal SubTotal { get; set; }
            public decimal Total { get; set; }

        }

        public class Expenses
        {
            public List<Expense> List { get; set; }
            public decimal Total { get; set; }
        }

        public class Incomes
        {
            public List<Income> List { get; set; }
            public decimal Total { get; set; }
        }

        public class Deposits
        {
            public List<UserPrePay> List { get; set; }
            public decimal Total { get; set; }
            public decimal TotalCash { get; set; }
            public decimal TotalCredit { get; set; }
        }

    }
}