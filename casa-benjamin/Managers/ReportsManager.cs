using casa_benjamin.Helpers;
using casa_benjamin.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using Dapper;
using MySql.Data.MySqlClient;
using System.Configuration;
using System.Threading.Tasks;
using System.Collections.Concurrent;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.BookKeeping.Entities;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.User.Repositories;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.User.Services;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.Kitchen.Services;

namespace casa_benjamin.Managers
{
    public sealed class ReportsManager
    {
        private static volatile ReportsManager instance;
        private static object syncRoot = new Object();

        private UserRepository userRepository;
        private GenericRepository genericRepository;

        private ReportsManager()
        {
            userRepository = new UserRepository();
            genericRepository = new GenericRepository();
        }

        public static ReportsManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new ReportsManager();
                    }
                }

                return instance;
            }
        }

        public Dictionary<string, object> GetPeriodData(ReportDateQuery dateQuery)
        {
            var data = new Dictionary<string, object>();
            List<User> periodUsers = new List<User>();

            List<CheckOut> periodCheckouts = new List<CheckOut>();
            List<StockItem> stockItems = new List<StockItem>();

            string startPeriodString = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriodString = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriodString = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriodString = dateQuery.ToOrDefault().AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            }            

            var residents = UserManager.Instance.GetResidents();
            if(residents.Count == 0)
            {
                residents.Add(new Resident
                {
                    id = -2,
                    user_id = -2
                });
            }

            periodUsers = genericRepository.Get<User>(string.Format("select * from user where date(cidate) between '{0}' and '{1}' and is_hidden is null", startPeriodString, endPeriodString)).ToList();

            dynamic periodKitchenOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and menu_category_type = {2} and user_id not in ({3});", startPeriodString, endPeriodString, (int)MenuCategoryType.Kitchen, string.Join(",", residents.Select(x => x.user_id).ToList()))).First();
            dynamic periodKitchenCashAndCreditOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and pay_type_id != 1 and menu_category_type = {2};", startPeriodString, endPeriodString, (int)MenuCategoryType.Kitchen)).First();
            dynamic periodKitchenCanceledOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 1 and menu_category_type = {2};", startPeriodString, endPeriodString, (int)MenuCategoryType.Kitchen)).First();
            List<dynamic> paymentKithcenBreakDown = genericRepository.Get<dynamic>(string.Format("SELECT pay_type_id, COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and menu_category_type = {2} group by pay_type_id;", startPeriodString, endPeriodString, (int)MenuCategoryType.Kitchen)).ToList();

            dynamic periodServicesOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and menu_category_type = {2} and user_id not in ({3});", startPeriodString, endPeriodString, (int)MenuCategoryType.Service, string.Join(",", residents.Select(x => x.user_id).ToList()))).First();
            dynamic periodServicesCashAndCreditOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and pay_type_id != 1 and menu_category_type = {2};", startPeriodString, endPeriodString, (int)MenuCategoryType.Service)).First();
            dynamic periodServicesCanceledOrders = genericRepository.Get<dynamic>(string.Format("SELECT COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 1 and menu_category_type = {2};", startPeriodString, endPeriodString, (int)MenuCategoryType.Service)).First();
            List<dynamic> paymentServicesBreakDown = genericRepository.Get<dynamic>(string.Format("SELECT pay_type_id, COALESCE(sum(total),0) total, count(*) cnt FROM menu_order where date(order_date) between '{0}' and '{1}' and is_canceled = 0 and menu_category_type = {2} group by pay_type_id;", startPeriodString, endPeriodString, (int)MenuCategoryType.Service)).ToList();

            
            periodCheckouts = genericRepository.Get<CheckOut>(string.Format("select * from check_outs where date(check_out_date) between '{0}' and '{1}'", startPeriodString, endPeriodString)).ToList();

            data["periodUsers"] = periodUsers;

            data["periodOrders"] = (long)periodKitchenOrders.cnt;
            data["periodOrdersSum"] = (double)periodKitchenOrders.total;
            data["periodCashAndCreditOrders"] = (long)periodKitchenCashAndCreditOrders.cnt;
            data["periodCashAndCreditOrdersSum"] = (double)periodKitchenCashAndCreditOrders.total;
            data["periodCanceledOrders"] = (long)periodKitchenCanceledOrders.cnt;
            data["periodCanceledOrdersSum"] = (double)periodKitchenCanceledOrders.total;

            data["periodServicesOrders"] = (long)periodServicesOrders.cnt;
            data["periodServicesOrdersSum"] = (double)periodServicesOrders.total;
            data["periodServicesCashAndCreditOrders"] = (long)periodServicesCashAndCreditOrders.cnt;
            data["periodServicesCashAndCreditOrdersSum"] = (double)periodServicesCashAndCreditOrders.total;
            data["periodServicesCanceledOrders"] = (long)periodServicesCanceledOrders.cnt;
            data["periodServicesCanceledOrdersSum"] = (double)periodServicesCanceledOrders.total;

            data["periodCheckouts"] = periodCheckouts;
            data["periodAccomodationSum"] = periodCheckouts.Sum(x => x.total_accommodation);

            data["kitchenRevenue"] = periodCheckouts.Sum(x => x.total_kitchen);
            data["servicesRevenue"] = periodCheckouts.Sum(x => x.total_services);


            data["periodCheckoutsTotal"] = periodCheckouts.Sum(x => x.total);
            data["totalRevenue"] = (double)data["periodCashAndCreditOrdersSum"] +
                                   (double)data["periodServicesCashAndCreditOrdersSum"] +
                                   (double)data["periodCheckoutsTotal"];

            data["ordersDebit"] = 0;
            data["ordersCash"] = 0;
            data["ordersCredit"] = 0;

            foreach (var item in paymentKithcenBreakDown)
            {
                switch ((int)item.pay_type_id)
                {

                    case (int)PayType.Cuenta:
                        data["ordersDebit"] = (double)item.total;
                        break;
                    case (int)PayType.Cash:
                        data["ordersCash"] = (double)item.total;
                        break;
                    case (int)PayType.Credit:
                        data["ordersCredit"] = (double)item.total;
                        break;
                }
            }

            data["ordersServicesDebit"] = 0;
            data["ordersServicesCash"] = 0;
            data["ordersServicesCredit"] = 0;

            foreach (var item in paymentServicesBreakDown)
            {
                switch ((int)item.pay_type_id)
                {

                    case (int)PayType.Cuenta:
                        data["ordersServicesDebit"] = (double)item.total;
                        break;
                    case (int)PayType.Cash:
                        data["ordersServicesCash"] = (double)item.total;
                        break;
                    case (int)PayType.Credit:
                        data["ordersServicesCredit"] = (double)item.total;
                        break;
                }
            }

            data["stayingGuests"] = 0;
            List<GuestCount> guestCountList = GetGuestsCount(dateQuery);
            data["stayingGuests"] = guestCountList.Count == 0 ? 0 : (guestCountList.Sum(x => x.count) / (double)guestCountList.Count);

            if (!dateQuery.Monthly)
            {
                //Get all the future revenue 
                ConcurrentBag<UIUserBill> usersBills = GetFutureRevenue();
                var residentsSum = GetResidentCost(dateQuery.From.GetValueOrDefault());
                data["residentsSum"] = residentsSum.HasValue ? residentsSum.Value : 0;
                data["accruedKitchenSum"] = usersBills.Sum(x => x.KitchenTotal);
                data["accruedServicesSum"] = usersBills.Sum(x => x.ServicesTotal);
                data["accruedAccomodationSum"] = usersBills.Sum(x => x.RoomTotal);
                data["accruedDiscountsSum"] = usersBills.Sum(x => x.DiscountTotal);
                data["accruedRevenue"] = usersBills.Sum(x => x.SubTotal) - (double)data["accruedDiscountsSum"] - Convert.ToDouble(data["residentsSum"]);
            }

            if (dateQuery.Monthly)
            {
                var demo = genericRepository.Get<DemographicsCount>(string.Format("SELECT count(*) cnt, nationality FROM `user` where date(cidate) between '{0}' and '{1}' and is_hidden is null group by nationality", startPeriodString, endPeriodString)).ToList();
                data["demographics"] = demo;
            }


            return data;
        }

        public List<Shift> GetShifts(ReportDateQuery dateQuery)
        {
            string startPeriodString = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriodString = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriodString = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriodString = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            string q = "";

                if (dateQuery.Monthly)
                {
                    q = string.Format(@"SELECT DATE_FORMAT(shift_date,'%Y-%m-01') shift_date,
		                                    sum(shift_total_cash) shift_total_cash,
                                            sum(shift_total_credit) shift_total_credit,
                                            sum(shift_total_canceled) shift_total_canceled,
                                            sum(shift_total) shift_total
                                    FROM shift_end where date(shift_date) between '{0}' and '{1}' 
                                    group by MONTH(shift_date),YEAR(shift_date);", startPeriodString, endPeriodString);
                }
                else
                {
                    q = string.Format(@"SELECT * FROM shift_end
                         where date(shift_date) between '{0}' and '{1}';", startPeriodString, endPeriodString);
                }            

            return genericRepository.Get<Shift>(q).ToList();
        }

        public List<OrderItems> GetOrderItemsCountList(MenuCategoryType catType, ReportDateQuery dateQuery)
        {
            string startPeriodString = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriodString = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriodString = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriodString = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            string q = "";
            if (dateQuery.To.HasValue)
            {
                q = string.Format(@"SELECT menu_item_id,menu_item_name,menu_category_name,count(*) total FROM order_items
                         where menu_category_type = {0} and date(order_date) >= '{1}' and date(order_date) <= '{2}' group by menu_item_id order by total desc;", (int)catType, startPeriodString, endPeriodString);

            }
            else
            {
                q = string.Format(@"SELECT menu_item_id,menu_item_name,menu_category_name,count(*) total FROM order_items
                         where menu_category_type = {0} group by menu_item_id order by total desc;", (int)catType);

            }

            return genericRepository.Get<OrderItems>(q).ToList();
        }

        public Dictionary<string,object> GetCheckOutsReport(ReportDateQuery dateQuery)
        {

            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            }

            var result = new Dictionary<string, object>();
           
            string q = "";
            q = string.Format(@"SELECT * from check_outs where date(check_out_date) between '{0}' and '{1}'", startPeriod, endPeriod);

            List<CheckOut> data = genericRepository.Get<CheckOut>(q).ToList();
            
            result["checkouts"] = data;
            result["maleCount"] = data.Count == 0 ? 0 : data.Count(x => x.user_sex);
            result["femaleCount"] = data.Count == 0 ? 0 : data.Count(x => !x.user_sex);
            result["total"] = data.Count == 0 ? 0 : data.Sum(x => x.total);
            result["totalOrders"] = data.Count == 0 ? 0 : data.Sum(x => x.total_kitchen);
            result["totalServices"] = data.Count == 0 ? 0 : data.Sum(x => x.total_services);
            result["totalAccomodation"] = data.Count == 0 ? 0 : data.Sum(x => x.total_accommodation);
            result["totalDiscount"] = data.Count == 0 ? 0 : data.Sum(x => x.total_discount);
            result["averageNights"] = data.Count == 0 ? 0 : (double)data.Sum(x => x.total_nights) / (double)data.Count;

            result["totalCash"] = data.Sum(x => x.total_cash);
            result["totalCredit"] = data.Sum(x => x.total_credit);
            result["averageSpend"] = data.Count == 0 ? 0 : ((double)result["total"] / (double)data.Count);

            return result;
           
        }

        public List<User> GetCheckinsReport(ReportDateQuery dateQuery)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            string q = "";
            q = string.Format(@"SELECT * from user where date(cidate) between '{0}' and '{1}'", startPeriod, endPeriod);

            return genericRepository.Get<User>(q).ToList();
        }

        public List<StockHistoryItem> GetStockReport(ReportDateQuery dateQuery, int itemID)
        {
            string startPeriod;
            string endPeriod;

            startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
            endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            

            string q = "";
            q = string.Format(@"SELECT * from stock_history where date(timestamp) between '{0}' and '{1}' and menu_item_id = {2}", startPeriod, endPeriod,itemID);

            return genericRepository.Get<StockHistoryItem>(q).ToList();
        }

        public List<StaffEvent> GetStaffEventsReport(ReportDateQuery dateQuery)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            string q = "";
            q = string.Format(@"SELECT * from staff_event where date(event_date) between '{0}' and '{1}'", startPeriod, endPeriod);

            return genericRepository.Get<StaffEvent>(q).ToList();
        }

        public List<Shift> GetShiftsOverTime(int daysFromNow)
        {
            return genericRepository.Get<Shift>("SELECT * FROM shift_end order by date(shift_date) desc limit " + daysFromNow).ToList();
        }

        public Dictionary<string,object> GetDemographicsReport(ReportDateQuery dateQuery)
        {
            var data = new Dictionary<string, object>();

            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            List<dynamic> maleKitchenDetails = genericRepository.Get<dynamic>(string.Format(@"SELECT items.menu_category_name,items.menu_item_name,sum(items.total) total, count(*) cnt FROM order_items items inner join user users 
                                                                            on items.user_id = users.id
                                                                            where date(items.order_date) between '{0}' and '{1}' and users.sex = {2} and items.menu_item_id != -1
                                                                            and items.menu_category_type = {3}
                                                                            group by items.menu_item_id;",startPeriod,endPeriod,1, (int)MenuCategoryType.Kitchen)).ToList();

            List<dynamic> femaleKitchenDetails = genericRepository.Get<dynamic>(string.Format(@"SELECT items.menu_category_name,items.menu_item_name,sum(items.total) total, count(*) cnt FROM order_items items inner join user users 
                                                                            on items.user_id = users.id
                                                                            where date(items.order_date) between '{0}' and '{1}' and users.sex = {2} and items.menu_item_id != -1
                                                                            and items.menu_category_type = {3}
                                                                            group by items.menu_item_id;", startPeriod, endPeriod, 0, (int)MenuCategoryType.Kitchen)).ToList();

            List<dynamic> maleServicesDetails = genericRepository.Get<dynamic>(string.Format(@"SELECT items.menu_category_name,items.menu_item_name,sum(items.total) total, count(*) cnt FROM order_items items inner join user users 
                                                                            on items.user_id = users.id
                                                                            where date(items.order_date) between '{0}' and '{1}' and users.sex = {2} and items.menu_item_id != -1
                                                                            and items.menu_category_type = {3}
                                                                            group by items.menu_item_id;", startPeriod, endPeriod, 1, (int)MenuCategoryType.Service)).ToList();

            List<dynamic> femaleServicesDetails = genericRepository.Get<dynamic>(string.Format(@"SELECT items.menu_category_name,items.menu_item_name,sum(items.total) total, count(*) cnt FROM order_items items inner join user users 
                                                                            on items.user_id = users.id
                                                                            where date(items.order_date) between '{0}' and '{1}' and users.sex = {2} and items.menu_item_id != -1
                                                                            and items.menu_category_type = {3}
                                                                            group by items.menu_item_id;", startPeriod, endPeriod, 0, (int)MenuCategoryType.Service)).ToList();

            List<CheckOut> checkouts = genericRepository.Get<CheckOut>(string.Format(@"SELECT * from check_outs where date(check_out_date) between '{0}' and '{1}'", startPeriod, endPeriod)).ToList();

            data["maleKitchenSum"] = 0;
            data["femaleKitchenSum"] = 0;

            data["maleServicesSum"] = 0;
            data["femaleServicesSum"] = 0;

            data["maleAccomodationSum"] = 0;
            data["femaleAccomodationSum"] = 0;

            data["maleTotalNight"] = 0;
            data["femaleTotalNight"] = 0;

            data["maleAvgNight"] = 0;
            data["femaleAvgNight"] = 0;

            data["malePopularKitchenItems"] = "";
            data["femalePopularKitchenItems"] = "";

            data["malePopularServicesItems"] = "";
            data["femalePopularServicesItems"] = "";



            return data;
        }

        public Expense GetExepnse(int id)
        {
           return  genericRepository.Get<Expense>("select * from expense where id=" + id).First();
        }

        public Income GetIncome(int id)
        {
            return genericRepository.Get<Income>("select * from income where id=" + id).First();
        }

        public void DeleteExepnse(Expense ex)
        {
            genericRepository.Delete(ex);
        }

        public void InsertExepnseLog(ExpenseLog el)
        {
            genericRepository.Insert(el);
        }

        public void UpdateExepnse(Expense ex)
        {
            genericRepository.Update(ex);
        }

        public long AddExepnse(Expense ex)
        {
            return genericRepository.Insert(ex);
        }

        public void InsertIncomeLog(IncomeLog il)
        {
            genericRepository.Insert(il);
        }

        public List<CashRegisterEvent> GetExepnsesOrIncomesByMonth(DateTime date,List<int> eventTypes)
        {
            return genericRepository.Get<CashRegisterEvent>(
                $@"SELECT event_type_id, sum(event_value) event_value, ifnull(event_realted_entity_id,-1) event_realted_entity_id FROM cash_register_event
                 where event_type_id in ({String.Join(",",eventTypes)}) and MONTH(event_date) = {date.Month} and YEAR(event_date) = {date.Year}
                 group by event_realted_entity_id;").ToList();
        }

        public Dictionary<DateTime,List<Expense>> GetExepnsesTable(DateTime from, DateTime to)
        {
            var dic = new Dictionary<DateTime, List<Expense>>();

            DateTime start = from;
            DateTime end = to;

            Finance.FinanceAdvisor advisor = new Finance.FinanceAdvisor();

            while (start <= end)
            {
                List<Expense> expenses = advisor.GetExpenses(start, start.AddMonths(1));                
                dic.Add(new DateTime(start.Year, start.Month, start.Day), expenses);
                start = start.AddMonths(1);
            }

            return dic;
        }

        public Dictionary<DateTime, List<Income>> GetIncomesTable(DateTime from, DateTime to)
        {
            var dic = new Dictionary<DateTime, List<Income>>();

            DateTime start = from;
            DateTime end = to;

            Finance.FinanceAdvisor advisor = new Finance.FinanceAdvisor();

            while (start <= end)
            {
                List<Income> table = advisor.GetIncomes(start, start.AddMonths(1));
                dic.Add(new DateTime(start.Year, start.Month, start.Day), table);
                start = start.AddMonths(1);
            }

            return dic;
        }

        public List<Expense> GetExepnsesDetailsTable(DateTime from, DateTime to, int id)
        {
            var result = new List<Expense>();
         
            string q = $@"SELECT * FROM expense
                 where report_date >= '{from.ToMySqlDateTimeString()}' and report_date < '{to.ToMySqlDateTimeString()}'                  
                    and expense_category_id = {id};";

            result = genericRepository.Get<Expense>(q).ToList();       

            return result;
        }

        public List<dynamic> GetDemographics(DateTime from, DateTime to)
        {
            var result = new List<dynamic>();

            string q = $@"SELECT count(*) cnt, nationality FROM `user`
                 where cidate >= '{from.ToMySqlDateTimeString()}' and cidate < '{to.ToMySqlDateTimeString()}'                  
                    group by nationality;";

            result = genericRepository.Get<dynamic>(q).ToList();

            return result;
        }

        public List<Income> GetIncomesDetailsTable(DateTime from, DateTime to, int id)
        {
            var result = new List<Income>();

            string q = $@"SELECT * FROM income
                 where report_date between '{from.ToMySqlDateTimeString()}' and '{to.ToMySqlDateTimeString()}'                 
                    and category_id = {id};";

            result = genericRepository.Get<Income>(q).ToList();

            return result;
        }

        public List<GuestCount> GetGuestsCount(ReportDateQuery dateQuery)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            return genericRepository.Get<GuestCount>(string.Format("select * from guest_count where guestcount_date between '{0}' and '{1}'",startPeriod,endPeriod)).ToList();
        }

        public long CalcGuestsCount(DateTime date)
        {
            using (var con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                object result = con.ExecuteScalar(string.Format("SELECT count(*) FROM user where is_hidden is null and is_resident = 0 and date(cidate) <= '{0}' and (codate is null or date(codate) > '{0}');", date.ToMySqlDateString()));
                return (long)result;
            }
        }

        public List<User> GetGuestsList(DateTime date)
        {
            using (var con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<User>(string.Format("SELECT * FROM user where is_hidden is null and date(cidate) <= '{0}' and (codate is null or date(codate) > '{0}');", date.ToMySqlDateString())).ToList();                
            }
        }

        public List<User> GetStayingGuestsList()
        {
            using (var con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<User>(string.Format("SELECT * FROM user where is_hidden is null and is_checked_out = 0 and is_resident = 0;")).ToList();
            }
        }

        public GuestCount GetLatestGuestCount()
        {
            return genericRepository.Get<GuestCount>("select * from guest_count order by id desc limit 1").FirstOrDefault();
        }

        public void InsertGuestCount(GuestCount gc)
        {
            using (var con = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Execute("insert into guest_count (count,guestcount_date) values(@count,@guestcount_date) ON DUPLICATE KEY update count=@count", new { count = gc.count, guestcount_date = gc.guestcount_date });
            }
        }
        
        public User GetFirstUser()
        {
            return genericRepository.Get<User>("select * from user order by id asc limit 1").FirstOrDefault();
        }

        public List<Order> GetOrders(ReportDateQuery dateQuery,MenuCategoryType type)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().AddDays(1).ToMySqlDateString();
        
            if (dateQuery.Monthly)
            {
                DateTime from = dateQuery.FromOrDefault();
                from = new DateTime(from.Year, from.Month, 1);
                DateTime to = dateQuery.ToOrDefault();
                to = new DateTime(to.Year, to.Month, 1);

                startPeriod = from.ToString("yyyy-MM-01");
                endPeriod = to.AddMonths(1).ToString("yyyy-MM-dd");
            }

            return genericRepository.Get<Order>(string.Format("select * from menu_order where order_date >= '{0}' and order_date < '{1}' and menu_category_type={2}", startPeriod, endPeriod,(int)type)).ToList();
        }

        public List<OrderRow> GetOrdersByStaff(ReportDateQuery dateQuery,int staffId)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateTimeString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateTimeString();

            List<Order> ordersIds = genericRepository.Get<Order>(string.Format("select id from menu_order where order_date >= '{0}' and order_date  <= '{1}' and staff_id={2}", startPeriod, endPeriod, staffId)).ToList();
            List<OrderRow> orders = KitchenManager.Instance.GetOrderItems(ordersIds.Select(x => x.id).ToList());
            return orders;
        }

        public List<OrderItems> GetOrdersItemsByMenuItemId(ReportDateQuery dateQuery, int menuItemId)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                DateTime from = dateQuery.FromOrDefault();
                from = new DateTime(from.Year, from.Month, 1);
                DateTime to = dateQuery.ToOrDefault();
                to = new DateTime(to.Year, to.Month, 1);

                startPeriod = from.ToString("yyyy-MM-01");
                endPeriod = to.AddMonths(1).AddDays(-1).ToString("yyyy-MM-dd");
            }

            return genericRepository.Get<OrderItems>(string.Format("select * from order_items where date(order_date) between '{0}' and '{1}' and menu_item_id={2}", startPeriod, endPeriod, menuItemId)).ToList();
        }

        public List<Order> GetOrdersByOrderId(List<int> ordersIds)
        {
            return genericRepository.Get<Order>(string.Format("select * from menu_order where id in ({0})", string.Join(",", ordersIds))).ToList();
        }

        public List<ExpenseCategory> GetExpenseCategories(bool onlyActive = false)
        {
            if (onlyActive)
            {
                return genericRepository.Get<ExpenseCategory>("select * from expense_category where is_active = 1 or is_active is null").ToList();
            }
            else
            {
                return genericRepository.Get<ExpenseCategory>("select * from expense_category").ToList();
            }
        }

        public List<IncomeCategory> GetIncomeCategories(bool onlyActive = false)
        {
            if (onlyActive)
            {
                return genericRepository.Get<IncomeCategory>("select * from income_category where is_active = 1 or is_active is null").ToList();
            }
            else
            {
                return genericRepository.Get<IncomeCategory>("select * from income_category").ToList();
            }
        }

        public ExpenseCategory GetExpenseCategory(int id)
        {
            return genericRepository.Get<ExpenseCategory>("select * from expense_category where id =" + id).FirstOrDefault();
        }

        public List<UserDiscount> UserDiscounts(ReportDateQuery dateQuery)
        {
            string startPeriod = dateQuery.FromOrDefault().ToMySqlDateString();
            string endPeriod = dateQuery.ToOrDefault().ToMySqlDateString();

            if (dateQuery.Monthly)
            {
                startPeriod = dateQuery.FromOrDefault().ToString("yyyy-MM-01");
                endPeriod = dateQuery.ToOrDefault().AddMonths(1).ToString("yyyy-MM-01");
            }

            return genericRepository.Get<UserDiscount>(string.Format("select * from user_discount  where date(discount_date) between '{0}' and '{1}'", startPeriod, endPeriod)).ToList();
        }

        public List<User> GetCheckOutsDue()
        {
            return genericRepository.Get<User>($"select * from user where is_checked_out = 0 and intended_codate <= '{DateTime.Now.ToMySqlDateString()}'").ToList();
        }

        public int GetCheckOutsDueCount()
        {
            object a = genericRepository.ExecuteScalar($"select count(*) from user where is_checked_out = 0 and intended_codate <= '{DateTime.Now.ToMySqlDateString()}'");
            return Convert.ToInt32(a);
        }

        public decimal GetTotalRevenue(DateTime date)
        {
            DateTime _date = new DateTime(date.Year, date.Month, 1);

            var data = GetPeriodData(new ReportDateQuery
            {
                From = _date,
                To = _date,
                Monthly = true
            });

            return Convert.ToDecimal(data["totalRevenue"]);
        }

        public ConcurrentBag<UIUserBill> GetFutureRevenue()
        {
            //Get all the future revenue 
            List<dynamic> userIds = genericRepository.Get<dynamic>("SELECT id FROM user where is_hidden is null and is_resident = 0 and is_checked_out = 0;").ToList();
            ConcurrentBag<UIUserBill> usersBills = new ConcurrentBag<UIUserBill>();
            Parallel.ForEach(userIds, (userid) =>
            {
                var userBill = UserManager.Instance.GetUserBill((int)userid.id);
                usersBills.Add(userBill);
            });

            return usersBills;
        }

        public double? GetResidentCost(DateTime date)
        {
            DateTime fromDate = new DateTime(date.Year, date.Month, 1);
            DateTime to = fromDate.AddMonths(1).AddDays(-1);

            List<Resident> residents = UserManager.Instance.GetResidents();
            if (residents.Count == 0) return 0;

            string q = string.Format("SELECT sum(total) total FROM menu_order where user_id in ({0}) and pay_type_id = 1 and is_canceled = 0 and date(order_date) >= '{1}' and date(order_date) <= '{2}';", string.Join(",", residents.Select(x => x.user_id).ToList()),fromDate.ToMySqlDateString(),to.ToMySqlDateString());
           return (double?)genericRepository.Get<dynamic>(q).First().total;
        }

    }


    public class PeriodDates
    {
        public DateTime start { get; set; }
        public DateTime end { get; set; }
        public string startString { get; set; }
        public string endString { get; set; }

    }

    public class DemographicsCount
    {
        public int cnt { get; set; }
        public string nationality { get; set; }
    }
}