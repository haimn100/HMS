using casa_benjamin.Finance.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using casa_benjamin.Helpers;
using MySql.Data.MySqlClient;
using System.Configuration;
using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Shared.Enums;

namespace casa_benjamin.Finance
{
    public class FinanceAdvisor : IFinanceAdvisor
    {
        private GenericRepository GenericRepository { get; set; }

        public FinanceAdvisor()
        {
            GenericRepository = new GenericRepository();
        }

        public DailyIncomeStatment GetDailyIncomeStatement(DateTime? from = null, DateTime? to = null)
        {
            from = from.HasValue ? from.Value : DateTime.Now; 
            to = to.HasValue ? to.Value : from.Value.AddDays(1); 

            string sqlFromDate = from.Value.ToMySqlDateString();
            string sqlToDate = to.Value.ToMySqlDateString();

            DailyIncomeStatment model = new DailyIncomeStatment();
            model.Date = from.Value;

            //CheckOuts
            Dictionary<DateTime,DailyIncomeStatment.CheckOuts> checkoutsReports = GetDailyCheckOut(sqlFromDate, sqlToDate);

            model.CheckOutsReport = checkoutsReports.Count == 0 ? new DailyIncomeStatment.CheckOuts() : checkoutsReports.First().Value;

            //Orders
            string query = GetDailyOrdersSumQuery(sqlFromDate, sqlToDate, "2,3");


            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                conn.Open();
                MySqlCommand cmd = new MySqlCommand(query, conn);
                MySqlDataReader reader = cmd.ExecuteReader();

                model.CashOrdersReport = new DailyIncomeStatment.CashOrders();
                model.CreditOrdersReport = new DailyIncomeStatment.CreditOrders();

                while (reader.Read() && !reader.IsDBNull(0))
                {

                    if (Convert.ToInt32(reader["cat"]) == (int)MenuCategoryType.Kitchen)
                    {
                        if (Convert.ToInt32(reader["pay_type_id"]) == (int)PayType.Cash)
                        {
                            model.CashOrdersReport.Kitchen = Convert.ToDecimal(reader["total"]);
                        }
                        else
                        {
                            model.CreditOrdersReport.Kitchen = Convert.ToDecimal(reader["total"]);
                        }
                    }
                    else
                    {
                        if (Convert.ToInt32(reader["pay_type_id"]) == (int)PayType.Cash)
                        {
                            model.CashOrdersReport.Services = Convert.ToDecimal(reader["total"]);
                        }
                        else
                        {
                            model.CreditOrdersReport.Services = Convert.ToDecimal(reader["total"]);
                        }
                    }
                    model.CashOrdersReport.Discount += Convert.ToDecimal(reader["discount"]);
                }
                model.CashOrdersReport.Total = model.CashOrdersReport.Kitchen + model.CashOrdersReport.Services;
                model.CashOrdersReport.SubTotal = model.CashOrdersReport.Total + model.CashOrdersReport.Discount;

                model.CreditOrdersReport.Total = model.CreditOrdersReport.Kitchen + model.CreditOrdersReport.Services;
                model.CreditOrdersReport.SubTotal = model.CreditOrdersReport.Total + model.CreditOrdersReport.Discount;
            }

            model.ExpensesReport = new DailyIncomeStatment.Expenses();
            model.ExpensesReport.List = GetExpenses(new DateTime(from.Value.Year, from.Value.Month, 1), new DateTime(from.Value.Year, from.Value.Month, 1).AddMonths(1))
                                                   .Where(x => x.expense_date.Day == from.Value.Day).ToList();
            model.ExpensesReport.Total = (decimal)model.ExpensesReport.List.Sum(x => x.expense_val);

            model.IncomesReport = new DailyIncomeStatment.Incomes();
            model.IncomesReport.List = GetIncomes(new DateTime(from.Value.Year, from.Value.Month, 1), new DateTime(from.Value.Year, from.Value.Month, 1).AddMonths(1))
                                                .Where(x => x.date.Day == from.Value.Day).ToList();
        
            model.IncomesReport.Total = model.IncomesReport.List.Sum(x => x.val);

            model.DepositsReport = new DailyIncomeStatment.Deposits();
            model.DepositsReport.List = GetDailyDeposits(from.Value, to.Value);
            model.DepositsReport.TotalCash = model.DepositsReport.List.Where(x=> x.pay_type == PayType.Cash || x.pay_type == 0).Sum(x => x.amount);
            model.DepositsReport.TotalCredit = model.DepositsReport.List.Where(x=> x.pay_type == PayType.Credit).Sum(x => x.amount + x.deposit_credit_card_charge);
            model.DepositsReport.Total = model.DepositsReport.List.Sum(x => x.amount + x.deposit_credit_card_charge);
            
            model.TotalRevenue = model.CheckOutsReport.TotalPaid +
                                 model.CashOrdersReport.Total +
                                 model.CreditOrdersReport.Total +
                                 model.DepositsReport.Total +
                                 model.IncomesReport.Total;
            model.TotalRevenue -= model.ExpensesReport.Total;
            return model;
        }

        public List<DailyIncomeStatment> MonthlyIncomeStatement(DateTime? from = null, DateTime? to = null)
        {
            var model = new List<DailyIncomeStatment>();
            DateTime _from = from.HasValue ? new DateTime(from.Value.Year, from.Value.Month, 1) : new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            DateTime _to = to.HasValue ? new DateTime(to.Value.Year, to.Value.Month, 1) : _from.AddMonths(1);
            string sqlFromDate = _from.ToMySqlDateString();
            string sqlToDate = _to.ToMySqlDateString();
                
            DateTime d = _from;
            while (d < _to)
            {
                DailyIncomeStatment dIncome = GetDailyIncomeStatement(d);
                model.Add(dIncome);
                d = d.AddDays(1);
            }


            return model;
        }

        private string GetDailyCheckoutsSumQuery(string from, string to)
        {
            string s = $@"SELECT date(check_out_date) _date,sum(total_kitchen) totalKitchen,
                   sum(total_services) totalServices,
                   sum(total_accommodation) totalAccom,
                   sum(total_cash) totalCash,
                   sum(total_credit) totalCredit,
                   sum(total_discount) totalDiscount,
                   sum(total_canceled) totalCanceled,
                   IFNULL(sum(cash_deposit),0) totalCashDeposit,
                   IFNULL(sum(credit_deposit),0) totalCreditDeposit,
                   IFNULL(sum(total_discount),0) totalDiscount,
                   IFNULL(sum(credit_charge_amount),0) totalCreditChargeAmount,
                   sum(total) total,
                   count(*) cnt
                   FROM check_outs where check_out_date >= '{from}' and check_out_date < '{to}'
                   group by _date
                   order by _date asc;";

            return s;
        }
        private string GetDailyOrdersSumQuery(string from, string to, string payTypes)
        {
            return $@"SELECT date(order_date) d,pay_type_id,menu_category_type,
                               ifnull(sum(total),0) total, 
                               ifnull(sum(discount),0) discount, 
                               menu_category_type cat, 
                               count(*) cnt 
                        FROM menu_order 
                        where pay_type_id in ({payTypes})
			            and is_canceled = 0
			            and order_date >= '{from}' and order_date <= '{to}'
                        group by d, menu_category_type,pay_type_id;";
        }
        private Dictionary<DateTime, DailyIncomeStatment.CheckOuts> GetDailyCheckOut(string from, string to)
        {
            var result = new Dictionary<DateTime, DailyIncomeStatment.CheckOuts>();
            using (MySqlConnection conn = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                conn.Open();

                MySqlCommand cmd = new MySqlCommand(GetDailyCheckoutsSumQuery(from, to), conn);
                MySqlDataReader checkOutsReport = cmd.ExecuteReader();

                while (checkOutsReport.Read() && !checkOutsReport.IsDBNull(0))
                {
                    var dailyCheckOut = new DailyIncomeStatment.CheckOuts
                    {
                        Total = Convert.ToDecimal(checkOutsReport["total"]),
                        TotalPaid = Convert.ToDecimal(checkOutsReport["totalCash"]) + Convert.ToDecimal(checkOutsReport["totalCredit"]),
                        TotalPeople = Convert.ToInt32(checkOutsReport["cnt"]),
                        TotalAccomodation = Convert.ToDecimal(checkOutsReport["totalAccom"]),
                        TotalKitchen = Convert.ToDecimal(checkOutsReport["totalKitchen"]),
                        TotalServices = Convert.ToDecimal(checkOutsReport["totalServices"]),
                        TotalDiscount = Convert.ToDecimal(checkOutsReport["totalDiscount"]),
                        TotalPaidInCash = Convert.ToDecimal(checkOutsReport["totalCash"]),
                        TotalPaidInCredit = Convert.ToDecimal(checkOutsReport["totalCredit"]),
                        TotalCanceled = Convert.ToDecimal(checkOutsReport["totalCanceled"]),
                        /* Altough the deposits data exists also in the checkouts table it should not
                           be included in the checkout report because its money paid in a different time
                           than the checkout time
                         */
                        //TotalCashDeposit = Convert.ToDecimal(checkOutsReport["totalCashDeposit"]),
                        //TotalCreditDeposit = Convert.ToDecimal(checkOutsReport["totalCreditDeposit"]),
                        TotalCreditChargeAmount = Convert.ToDecimal(checkOutsReport["totalCreditChargeAmount"])
                    };
                
                    result.Add(checkOutsReport.GetDateTime("_date"), dailyCheckOut);
                }
            }
            return result;
        }

        public List<Expense> GetExpenses(DateTime from, DateTime to)
        {
            return new  GenericRepository().Get<Expense>($"select * from expense where report_date >= '{from.ToMySqlDateString()}' and report_date < '{to.ToMySqlDateString()}'").ToList();
        }

        public static List<Expense> GetExpenses(DateTime from, DateTime to, List<int> categories)
        {
            from = new DateTime(from.Year, from.Month,1);
            to = new DateTime(to.Year, to.Month,1);
            return new GenericRepository().Get<Expense>($"select * from expense where expense_category_id in({string.Join(",",categories)}) and report_date >= '{from.ToMySqlDateString()}' and report_date < '{to.ToMySqlDateString()}'").ToList();
        }

        public List<Income> GetIncomes(DateTime from, DateTime to)
        {
            return new GenericRepository().Get<Income>($"select * from income where report_date >= '{from.ToMySqlDateString()}' and report_date < '{to.ToMySqlDateString()}'").ToList();
        }

        public static List<Income> GetIncomes(DateTime from, DateTime to, List<int> categories)
        {
            return new GenericRepository().Get<Income>($"select * from income where expense_category_id in({string.Join(",", categories)}) and report_date >= '{from.ToMySqlDateString()}' and report_date < '{to.ToMySqlDateString()}'").ToList();
        }

        public List<UserPrePay> GetDailyDeposits(DateTime from, DateTime to)
        {
            return GenericRepository.Get<UserPrePay>($"select * from user_prepay where prepay_date >= '{from.ToMySqlDateString()}' and prepay_date < '{to.ToMySqlDateString()}'").ToList();
        }

        public List<UserPrePay> GetMonthlyDeposits(int month, int year,List<int> excludeUsers = null)
        {
            if(excludeUsers != null)
            {
                return GenericRepository.Get<UserPrePay>($"select * from user_prepay where month(prepay_date) = {month} and year(prepay_date) = {year} and user_id not in ({string.Join(",",excludeUsers)})").ToList();
            }
            else
            {
                return GenericRepository.Get<UserPrePay>($"select * from user_prepay where month(prepay_date) = {month} and year(prepay_date) = {year}").ToList();

            }
        }


    }

    public interface IFinanceAdvisor
    {
        DailyIncomeStatment GetDailyIncomeStatement(DateTime? from, DateTime? to);
        List<Expense> GetExpenses(DateTime from, DateTime to);
        List<Income> GetIncomes(DateTime from, DateTime to);
        List<UserPrePay> GetDailyDeposits(DateTime from, DateTime to);
        List<UserPrePay> GetMonthlyDeposits(int month,int year, List<int> excludeUsers = null);
        List<DailyIncomeStatment> MonthlyIncomeStatement(DateTime? from = null, DateTime? to = null);
    }
}