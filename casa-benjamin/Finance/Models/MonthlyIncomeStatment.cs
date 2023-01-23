using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.CashRegister.Entities;
using System;
using System.Collections.Generic;

namespace casa_benjamin.Finance.Models
{
    /// <summary>
    /// Holds periodic income statment
    /// </summary>
    public class MonthlyIncomeStatment
    {

        public List<DailyIncomeRow> DailyIncomeRows { get; set; }
        public List<ExpenseRow> ExpensesRows { get; set; }

        public List<CashRegisterEvent> KnownExpensesEvents { get; set; }
        public List<CashRegisterEvent> UnKnownExpensesEvents { get; set; }
        public List<CashRegisterEvent> UnKnownIncomeEvents { get; set; }

        public decimal Revenue { get; set; }

        public decimal CurrentExpenses { get; set; }
        public decimal KitchenExpenses { get; set; }
        public decimal CleaningExpenses { get; set; }
        public decimal OtherExpenses { get; set; }

        public decimal AccommodationIncome { get; set; }
        public decimal KitchenIncome { get; set; }
        public decimal ServicesIncome { get; set; }
        public decimal DepositsIncome { get; set; }
        public decimal DiscountsIncome { get; set; }
        public decimal CreditChargeIncome { get; set; }
        public decimal OtherIncome { get; set; }
        public decimal TotalIncomeBreakDown { get; set; }

        public decimal MonthlyIncomes { get; set; }
        public decimal MonthlyExpenses { get; set; }
        public decimal MonthlyCashCheckouts { get; set; }
        public decimal MonthlyCreditCheckouts { get; set; }

        public decimal MonthlyKitchenCashOrders { get; set; }
        public decimal MonthlyKitchenCreditOrders { get; set; }

        public decimal MonthlyServicesCashOrders { get; set; }
        public decimal MonthlyServicesCreditOrders { get; set; }

        public decimal MonthlyAccommodation { get; set; }
        public decimal MonthlyDiscounts { get; set; }


        public class DailyIncomeRow
        {
            public DateTime Date { get; set; }
            public decimal Revenue { get; set; }
            public decimal Incomes { get; set; }
            public decimal Expneses { get; set; }
            public decimal CashCheckOuts { get; set; }
            public decimal CreditCheckOuts { get; set; }
            public decimal CashOrders { get; set; }
            public decimal CreditOrders { get; set; }
            public decimal Accommodation { get; set; }
            public decimal Deposits { get; set; }
        }
        public class ExpenseRow
        {
            public ExpenseCategory ExpenseCategory { get; set; }
            public decimal Expense { get; set; }
        }

    }
}