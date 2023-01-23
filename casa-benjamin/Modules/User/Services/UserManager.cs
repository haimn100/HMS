using casa_benjamin.Helpers;
using casa_benjamin.Models; 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using MySql.Data.MySqlClient;
using System.Configuration;
using Dapper;
using casa_benjamin.Internalization;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.App.Values;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.Kitchen.Services;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Repositories;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Entities;

namespace casa_benjamin.Modules.User.Services
{
    public sealed class UserManager
    {
        private static volatile UserManager instance;
        private static object syncRoot = new Object();

        private UserRepository userRepository;
        private GenericRepository genericRepository;


        private UserManager()
        {
            userRepository = new UserRepository();
            genericRepository = new GenericRepository();
        }

        public static UserManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new UserManager();
                    }
                }

                return instance;
            }
        }

        public Entities.User GetUser(string email)
        {
            return userRepository.GetUser(email);
        }

        public Entities.User GetUserByBarcode(string barcode)
        {
            return genericRepository.Get<Entities.User>("select * from user where is_checked_out = 0 and barcode = '" + barcode + "'").FirstOrDefault();
        }


        public List<Entities.User> GetUsers(List<int> ids)
        {
            return genericRepository.Get<Entities.User>(string.Format("select * from user where id in ({0})", string.Join(",", ids))).ToList();
        }

        public List<Entities.User> GetActiveUsers()
        {
            return genericRepository.Get<Entities.User>(string.Format("select * from user where is_hidden is null and is_resident = 0 and is_checked_out = 0")).ToList();
        }

        public List<Order> GetCuentaOrders(List<int> usersIds)
        {
            return genericRepository.Get<Order>($"select * from menu_order where user_id in ({string.Join(",", usersIds)}) and is_canceled = 0 and pay_type_id = 1").ToList();
        }

        public List<Entities.User> GetImmigrationUsers(DateTime date)
        {
            return genericRepository.Get<Entities.User>(string.Format("select * from user where is_hidden is null and (date(cidate) = '{0}' or date(codate) = '{0}')", date.ToMySqlDateString())).ToList();
        }


        public Staff.Entities.Staff GetStaff(string email)
        {
            return userRepository.GetStaff(email);
        }

        public List<Staff.Entities.Staff> GetStaff(bool onlyEmployees)
        {
            if (onlyEmployees)
            {
                return CacheManager.Instance.Staff.Where(x => x.type != UserType.Admin).ToList();
            }
            else
            {
                return CacheManager.Instance.Staff;
            }
        }

        public List<Modules.Staff.Entities.Staff> GetAllStaff()
        {
            return genericRepository.Get<Modules.Staff.Entities.Staff>("select * from staff");
        }

        public Staff.Entities.Staff GetStaff(int id)
        {
            return userRepository.GetStaff(id);
        }

        public bool DeleteStaff(int id)
        {
            return userRepository.DeleteStaff(id);
        }

        public void DeleteUser(int id)
        {
            using (var transactionScope = new TransactionScope())
            {
                using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
                {
                    con.Open();
                    con.Execute(string.Format("delete from check_outs where user_id = {0};", id));
                    con.Execute(string.Format("delete from order_items where user_id = {0};", id));
                    con.Execute(string.Format("delete from menu_order where user_id = {0};", id));
                    con.Execute(string.Format("delete from user_discount where user_id = {0};", id));
                    con.Execute(string.Format("delete from user_bed where user_id = {0};", id));
                    con.Execute(string.Format("delete from user where id = {0};", id));

                }
                transactionScope.Complete();
            }
        }

        public User.Entities.User GetUser(int id)
        {
            return userRepository.GetUser(id);
        }

        public List<Entities.User> GetUsersByDays(int days)
        {
            var now = DateTime.Now.AddDays(1).ToMySqlDateString();
            var till = DateTime.Now.AddDays(-days).ToMySqlDateString();
            return genericRepository.Get<Entities.User>(string.Format("select * from user where cidate >= '{0}' and cidate <= '{1}' and is_hidden is null", till, now)).ToList();
        }

        public User.Entities.User GetUserByPassport(string passport)
        {
            return userRepository.GetUserByPassport(passport);
        }

        public User.Entities.User CheckInUser(Entities.User user, int bedPrice, string comment)
        {
            long id = userRepository.InsertGuest(user, bedPrice, comment);
            return userRepository.GetUser((int)id);
        }

        public void CheckOutUser(int uid, int staffId, int totalCash, int totalCredit, decimal creditChargePercentage)
        {
            if (UserHelper.IsResident(uid))
            {
                User.Entities.User resident = GetUser(uid);
                resident.is_checked_out = true;
                resident.codate = DateTime.Now;
                UpdateUser(resident);
                return;
            }

            UIUserBill userBill = GetUserBill(uid);

            if (userBill.Total <= 0)
            {
                totalCash = 0;
                totalCredit = 0;
            }
            else if ((totalCash + totalCredit) != userBill.Total)
            {
                throw new Exception("payed amount is different from total");
            }


            var user = userBill.User;
            user.codate = DateTime.Now;
            Staff.Entities.Staff staff = GetStaff(staffId);
            staff = staff == null ? new Staff.Entities.Staff { id = 0, name = "" } : staff;

            UserBed lastUserBed = instance.GetUserBeds(uid).Last();
            decimal totalDepositsCreditCardCharge = userBill.TotalDepositsCreditCardCharge;

            var checkOutModel = new CheckOut
            {
                check_in_date = user.cidate,
                check_out_date = user.codate.Value,
                user_id = user.id,
                bed_id = user.bed_id,
                total_nights = userBill.Rooms.Sum(x => x.Nights),
                total_kitchen = userBill.KitchenTotal,
                total_services = userBill.ServicesTotal,
                total_accommodation = userBill.RoomTotal,
                total_cash = totalCash,
                total_credit = totalCredit,
                total_debit = userBill.Orders
                             .Where(x => !x.Order.is_canceled && x.Order.pay_type_id == PayType.Cuenta)
                             .Sum(s => s.Order.total),
                total_discount = userBill.DiscountTotal,
                total_canceled = userBill.Orders
                             .Where(x => x.Order.is_canceled)
                             .Sum(s => s.Order.total),
                total = userBill.Total,
                price_per_night = lastUserBed.price,
                user_sex = user.sex,
                staff = staff.name,
                user_name = user.name
            };

            /* If we charged credit card extra and we are not going to add to
             * the register by setting it in app settings we add it to the credit card total
            */
            if (totalCredit > 0 && creditChargePercentage > 0 && !CacheManager.Instance.AppSettings.AddCreditCardChargeToRegister)
            {
                double creditCardCharge = GetCreditCardCharge(totalCredit, creditChargePercentage, totalDepositsCreditCardCharge);
                checkOutModel.credit_charge_amount = (decimal)creditCardCharge;
                checkOutModel.credit_charge_percentage = creditChargePercentage;
                checkOutModel.total_credit += creditCardCharge;
                checkOutModel.total += creditCardCharge;
            }

            if (userBill.Deposits != null && userBill.Deposits.Count > 0)
            {
                /* Not Adding the deposit to the total since 
                   we reported the deposit as income at the time it was conceived 
                   hence, the total paid at the checkout was without the deposit
                 */
                //checkOutModel.total += userBill.Deposits.Sum(x=> x.amount);
                foreach (var deposit in userBill.Deposits)
                {
                    if (deposit.pay_type == PayType.Cash)
                    {
                        checkOutModel.cash_deposit += deposit.amount;
                    }
                    else
                    {
                        checkOutModel.credit_deposit += deposit.amount;
                    }
                }
            }

            long checkoutId = genericRepository.Insert(checkOutModel);

            //Insert cash to register
            if (totalCash > 0)
            {
                var lastCashEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastCashEvent.current_register_amount + totalCash,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = user.id,
                    event_type_id = EventType.CashRegisterAddFromCheckOut,
                    event_value = totalCash,
                    staff_id = staff.id,
                    staff_name = staff.name
                });
            }

            //If we charged credit card extra and we need to add it to cash register
            if (totalCredit > 0 && creditChargePercentage > 0 && CacheManager.Instance.AppSettings.AddCreditCardChargeToRegister)
            {
                double creditCardCharge = GetCreditCardCharge(totalCredit, creditChargePercentage, totalDepositsCreditCardCharge);
                var lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastEvent.current_register_amount + (decimal)creditCardCharge,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = user.id,
                    event_type_id = EventType.CashRegisterAddFromCheckOut,
                    event_value = (decimal)creditCardCharge,
                    staff_id = staff.id,
                    staff_name = staff.name,
                    comment = "Credit card charge of sum: " + totalCredit
                });
                checkOutModel.total_cash += creditCardCharge;
                checkOutModel.total += creditCardCharge;
            }

            user.is_checked_out = true;
            userRepository.UpdateUser(user);
            List<UserBed> userBeds = userRepository.GetUserBeds(uid);
            UserBed lastBed = userBeds.Last();
            lastBed.end_date = user.codate.Value;
            genericRepository.Update(lastBed);

            try
            {
                var uiRoom = CacheManager.Instance.Rooms.FirstOrDefault(x => x.beds.Select(y => y.bed_id).Contains(lastBed.bed_id));
                Room room = genericRepository.Get<Room>("select * from room where id = " + uiRoom.room.id).First();

                if (!room.is_clean_required && !room.is_cleaning_inspection_required)
                {
                    room.is_cleaning_inspection_required = true;
                    room.is_cleaning_inspection_date = DateTime.Now;
                    genericRepository.Update(room);
                }

                RoomBed _roomBed = uiRoom.beds.First(x => x.bed_id == lastUserBed.bed_id);
                _roomBed.last_checkout = DateTime.Now;
                genericRepository.Update(_roomBed);
            }
            catch { }
        }

        public User.Entities.User GetUserByBed(int bedId)
        {
            return userRepository.GetUserByBed(bedId);
        }

        public User.Entities.User GetActiveUser(string passport)
        {
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                return con.Query<Entities.User>("select * from user where is_checked_out = 0 and passport = @passport", new { passport }).FirstOrDefault();
            }
        }

        public List<Reservation> GetReservationsInDates(DateTime from, DateTime to, int roomId)
        {
            string _from = from.ToMySqlDateString();
            string _to = to.ToMySqlDateString();
            using (MySqlConnection con = new MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Open();
                string query = string.Format(@"
                                            SELECT * FROM reservation where 
                                            res_date >= '{0}' and res_date <= '{1}'
                                            and room_id = {2}
                                            ", _from, _to, roomId);
                var result = con.Query<Reservation>(query).ToList();
                return result;
            }
        }

        public bool MoveGuest(int uid, int destBed, int bedPrice, string comment)
        {
            if (userRepository.GetUserByBed(destBed) != null)
            {
                throw new Exception("Bed is already taken");
            }

            var bedCount = genericRepository.ExecuteScalar("select count(*) from room_bed where bed_id =" + destBed);
            if (Convert.ToInt32(bedCount) == 0)
            {
                throw new Exception("Bed does not exist");
            }

            userRepository.MoveGuest(uid, destBed, bedPrice, comment);

            CacheManager.Instance.RefreshUsers();
            CacheManager.Instance.RefreshRooms();
            return true;
        }

        public bool UpdateBed(Bed bed)
        {
            return userRepository.UpdateBed(bed);
        }

        public bool UpdateUserBed(UserBed bed)
        {
            return genericRepository.Update(bed);
        }

        public bool UpdateRoom(Room room)
        {
            return genericRepository.Update(room);
        }

        public bool UpdateRoomBed(RoomBed item)
        {
            return genericRepository.Update(item);
        }

        public bool UpdateUser(Entities.User user)
        {
            return genericRepository.Update(user);
        }

        public bool DeleteRoomBed(RoomBed item)
        {
            return genericRepository.Delete(item);
        }

        public long InsertRoomBed(RoomBed item)
        {
            return genericRepository.Insert(item);
        }

        public bool UpdateSettings(AppSettings settings)
        {
            return genericRepository.Update(settings);
        }

        public List<Bed> GetBeds()
        {
            return genericRepository.Get<Bed>("select * from bed").ToList();
        }

        public Bed GetBed(int bedId)
        {
            return genericRepository.Get<Bed>("select * from bed where id = " + bedId).First();
        }

        public List<Room> GetRooms()
        {
            return genericRepository.Get<Room>("select * from room").ToList();
        }

        public Room GetRoom(int roomId)
        {
            return genericRepository.Get<Room>("select * from room where id = " + roomId).First();
        }

        public List<RoomBed> GetRoomBeds(int roomId)
        {
            return genericRepository.Get<RoomBed>("select * from room_bed where room_id = " + roomId).ToList();
        }

        public List<RoomBed> GetRoomBeds()
        {
            return genericRepository.Get<RoomBed>("select * from room_bed").ToList();
        }

        public RoomBed GetRoomBed(int bedId)
        {
            return genericRepository.Get<RoomBed>("select * from room_bed where bed_id = " + bedId).FirstOrDefault();
        }

        public long AddStaff(Staff.Entities.Staff member)
        {
            member.password = CryptographyHelper.HashPassword(member.password);
            return userRepository.InsertStaff(member);
        }

        public bool UpdateStaff(Staff.Entities.Staff member)
        {
            return genericRepository.Update(member);
        }

        public List<UserBed> GetUserBeds(int userId)
        {
            return genericRepository.Get<UserBed>("select * from user_bed where user_id = " + userId).ToList();
        }

        public List<UserBed> GetUserBeds()
        {
            return genericRepository.Get<UserBed>("select * from user_bed where end_date is null").ToList();
        }

        public List<UserBed> GetAllUserBeds()
        {
            return genericRepository.Get<UserBed>("select * from user_bed").ToList();
        }

        public Shift GetLastShift()
        {
            return userRepository.GetLastShift();
        }

        public Shift GetShift(int id)
        {
            return genericRepository.Get<Shift>("select * from shift_end where id = " + id).FirstOrDefault();
        }

        public List<Order> GetShiftOrders(DateTime endOfShiftDate, DateTime? lastShiftDate)
        {
            return userRepository.GetShiftCashAndCreditOrders(endOfShiftDate, lastShiftDate);
        }

        public void EndShift(UIEndShift uiShift)
        {
            var employee = Instance.GetStaff(uiShift.staff);
            Shift shiftToInsert = new Shift
            {
                shift_date = uiShift.endshiftdate,
                shift_employee_id = employee == null ? 0 : employee.id,
                shift_employee_name = employee == null ? "" : employee.name,
                shift_total_cash = uiShift.totalcash,
                shift_total = uiShift.total,
                shift_total_credit = uiShift.totalcredit,
                shift_total_canceled = uiShift.totalcanceled,
                shift_total_checkouts = uiShift.totalcheckouts,
                shift_total_kitchen = uiShift.totalkitchen,
                shift_total_services = uiShift.totalservices,
                checkouts_cash = uiShift.checkoutscash,
                checkouts_credit = uiShift.checkoutscredit,
                expenses = uiShift.expenses,
                incomes = uiShift.incomes
            };

            long id = genericRepository.Insert(shiftToInsert);

            //reset cashier
            CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
            {
                event_type_id = EventType.CashRegisterReset,
                event_value = 0,
                comment = I18n.T("Reset from last shift"),
                event_date = DateTime.Now,
                current_register_amount = 0,
                staff_id = employee.id,
                staff_name = employee.name,
                event_realted_entity_id = (int)id
            });
        }

        public void InsertStaffEvent(StaffEvent e)
        {
            e.event_date = DateTime.Now;
            genericRepository.Insert(e);
        }

        public List<UserDiscount> GetUserDiscountsByUserIdForBill(int user_id)
        {
            return genericRepository.Get<UserDiscount>("select * from user_discount where user_id = " + user_id + " and payment_type_id = 1").ToList();
        }

        public List<UserDiscount> GetGhostUserDiscounts(DateTime from, DateTime to)
        {
            return genericRepository.Get<UserDiscount>(string.Format("select * from user_discount where discount_date between '{0}' and '{1}' and user_id = {2}", from.ToMySqlDateTimeString(), to.ToMySqlDateTimeString(), CacheManager.Instance.GhostUser.id)).ToList();
        }

        public UserDiscount GetUserDiscountByDiscountId(int id)
        {
            return genericRepository.Get<UserDiscount>("select * from user_discount where id = " + id).First();
        }

        public void InsertUserDiscount(UserDiscount ud)
        {
            genericRepository.Insert(ud);
        }

        public void DeleteUserDiscount(UserDiscount ud)
        {
            genericRepository.Delete(ud);
        }

        public UIUserBill GetUserBill(int userId)
        {
            var userBill = new UIUserBill
            {
                User = UserManager.Instance.GetUser(userId),
                Orders = new List<OrderRow>(),
                Rooms = new List<RoomRow>(),
                Discounts = new List<UserDiscount>()
            };

            userBill.Orders = new List<OrderRow>();

            List<Order> orders = KitchenManager.Instance.GetOrders(userId);
            foreach (var item in orders)
            {
                var orderRow = new OrderRow();
                orderRow.Order = item;
                orderRow.OrderItems = KitchenManager.Instance.GetOrderItems(item.id);
                userBill.Orders.Add(orderRow);
                if (item.split_total > 0)
                {
                    if (!item.is_canceled && item.pay_type_id == PayType.Cuenta)
                    {
                        userBill.OrdersTotal += item.total;
                    }

                    User.Entities.User splitedBy = item.splited_by == userId ? userBill.User : UserManager.Instance.GetUser(item.splited_by);
                    if (splitedBy != null)
                    {
                        orderRow.SplitedByUserId = splitedBy.id;
                        orderRow.SplitedByUserName = splitedBy.name;
                    }

                }
                else
                {
                    if (!item.is_canceled && item.pay_type_id == PayType.Cuenta)
                    {
                        userBill.OrdersTotal += item.total;
                    }
                }
            }

            var userBeds = GetUserBeds(userId);
            if (!userBill.User.is_checked_out)
            {
                userBeds.Last().end_date = DateTime.Now;
            }

            CalculateRoomPricesNew(userBill, userBeds);
            userBill.UserBeds = userBeds;

            userBill.KitchenTotal = userBill.Orders
                                    .Where(x => !x.Order.is_canceled && x.Order.menu_category_type == MenuCategoryType.Kitchen && x.Order.pay_type_id == PayType.Cuenta)
                                    .Sum(s => s.Order.total);

            userBill.ServicesTotal = userBill.Orders
                                .Where(x => !x.Order.is_canceled && x.Order.menu_category_type == MenuCategoryType.Service && x.Order.pay_type_id == PayType.Cuenta)
                                .Sum(s => s.Order.total);

            userBill.Discounts = GetUserDiscountsByUserIdForBill(userId);
            if (userBill.Discounts != null && userBill.Discounts.Count > 0)
            {
                /*
                 * This is a fix. all the orders that equal to 0 are discounts 
                 * that was made without any order, that means that its just to substract money from the User.Entities.User bill
                 * this should not be like that, so it's a temp hack
                 */
                userBill.Discounts = userBill.Discounts.Where(x => x.order_id == 0).ToList();
            }

            userBill.Deposits = Instance.GetUserDeposits(userId);
            userBill.TotalDepositsCreditCardCharge = userBill.Deposits.Sum(x => x.deposit_credit_card_charge);
            userBill.DiscountTotal = userBill.Discounts.Sum(x => x.price);
            userBill.SubTotal = userBill.OrdersTotal + userBill.RoomTotal;
            userBill.Total = userBill.OrdersTotal + userBill.RoomTotal - userBill.DiscountTotal;

            if (userBill.Deposits != null && userBill.Deposits.Count > 0)
            {
                userBill.Total -= userBill.Deposits.Sum(x => x.amount);
            }
            return userBill;
        }

        public CheckOut GetCheckOut(int userId)
        {
            return genericRepository.Get<CheckOut>("select * from check_outs where user_id = " + userId + " order by id desc limit 1").First();
        }

        public List<UserDiscount> GetUserDiscountByOrderId(int orderId)
        {
            return genericRepository.Get<UserDiscount>("select * from user_discount where order_id=" + orderId).ToList();
        }

        public List<Resident> GetResidents()
        {
            return genericRepository.Get<Resident>("select * from resident").ToList();
        }

        public Resident GetResident(int userid)
        {
            return genericRepository.Get<Resident>("select * from resident where user_id =" + userid).FirstOrDefault();
        }

        public List<CheckOut> GetCheckouts(DateTime from, DateTime to)
        {
            string startPeriod = from.ToMySqlDateTimeString();
            string endPeriod = to.ToMySqlDateTimeString();

            string q = "";
            q = string.Format(@"SELECT * from check_outs where check_out_date between '{0}' and '{1}'", startPeriod, endPeriod);

            return genericRepository.Get<CheckOut>(q).ToList();
        }

        public List<CheckOut> GetCheckoutsByMonth(DateTime date)
        {
            string startPeriod = new DateTime(date.Year, date.Month, 1).ToMySqlDateString();
            DateTime next = date.AddMonths(1);
            string endPeriod = new DateTime(next.Year, next.Month, 1).ToMySqlDateString();

            string q = "";
            q = string.Format(@"SELECT * from check_outs where check_out_date > '{0}' and check_out_date < '{1}'", startPeriod, endPeriod);

            return genericRepository.Get<CheckOut>(q).ToList();
        }

        public void AddResident(int userId)
        {
            Resident resident = GetResident(userId);
            if (resident == null)
            {
                genericRepository.Insert(new Resident
                {
                    user_id = userId
                });
            }
            User.Entities.User user = GetUser(userId);
            user.is_resident = true;
            UpdateUser(user);
        }

        public void RemoveResident(int userId)
        {
            Resident res = GetResident(userId);
            genericRepository.Delete(res);
            User.Entities.User user = GetUser(userId);
            user.is_resident = false;
            UpdateUser(user);
        }

        public void UpdateCheckOut(CheckOut checkout)
        {
            genericRepository.Update(checkout);
        }

        public double GetCreditCardCharge(double creditTotal, decimal creditChargePercentage, decimal depositCreditCardFee)
        {
            if (creditChargePercentage == 0)
            {
                return 0;
            }

            return ((creditTotal * (double)(creditChargePercentage / 100)) + (double)depositCreditCardFee);
        }

        public List<UserPrePay> GetUserDeposits(int userId)
        {
            return genericRepository.Get<UserPrePay>("select * from user_prepay where user_id = " + userId).ToList();
        }

        public UserPrePay GetUserPrePayById(int id)
        {
            return genericRepository.Get<UserPrePay>("select * from user_prepay where id = " + id).FirstOrDefault();
        }

        public void CalculateRoomPricesNew(UIUserBill userBill, List<UserBed> userBeds)
        {
            userBill.Nights = new List<RoomNight>();

            try
            {
                if (IsLessThanMinimumStay(userBeds))
                {
                    userBill.Rooms.Add(new RoomRow
                    {
                        StartDate = userBeds.First().start_date,
                        EndDate = userBeds.First().end_date.Value,
                        BedId = userBeds.First().bed_id,
                        Nights = 0,
                        Price = 0,
                        RoomNumber = CacheManager.Instance.GetRoomBed(userBeds.First().bed_id).room_id
                    });
                    return;
                }

                int checkoutTime = GetCheckoutTime();

                DateTime start = new DateTime(userBeds.First().start_date.Year, userBeds.First().start_date.Month, userBeds.First().start_date.Day);
                DateTime end = new DateTime(userBeds.Last().end_date.Value.Year, userBeds.Last().end_date.Value.Month, userBeds.Last().end_date.Value.Day);

                //if someone has entered at night or morning charge them another night
                //Example: guest enters at 3am at 1/1/2018 and checkout at 2/1/2018 he will be charged for 2 days
                if (userBeds.First().start_date.Hour < 6)
                {
                    start = start.AddDays(-1);
                }

                while (start <= end)
                {
                    userBill.Nights.Add(new RoomNight { Date = start, Price = userBeds.First().price, BedId = userBeds.First().bed_id, FullNight = true });
                    start = start.AddDays(1);
                }

                if (userBeds.Last().end_date.Value.Hour < checkoutTime)
                {
                    userBill.Nights.Last().FullNight = false;
                }

                foreach (var item in userBeds)
                {
                    DateTime temp_start = new DateTime(item.start_date.Year, item.start_date.Month, item.start_date.Day);
                    DateTime temp_end = new DateTime(item.end_date.Value.Year, item.end_date.Value.Month, item.end_date.Value.Day);

                    while (temp_start <= temp_end)
                    {
                        var roomNight = userBill.Nights.FirstOrDefault(x => x.Date == temp_start);
                        if (roomNight != null)
                        {
                            roomNight.Price = item.price;
                            roomNight.BedId = item.bed_id;
                        }
                        temp_start = temp_start.AddDays(1);
                    }
                }

                var groupedNights = userBill.Nights.GroupBy(x => new { x.BedId, x.Price });

                foreach (var item in groupedNights)
                {
                    var roomBed = CacheManager.Instance.GetRoomBed(item.First().BedId);
                    userBill.Rooms.Add(new RoomRow
                    {
                        StartDate = item.First().Date,
                        EndDate = item.Last().Date,
                        BedId = item.First().BedId,
                        Nights = item.Where(x => x.FullNight).Count(),
                        Price = item.First().Price,
                        RoomNumber = roomBed != null ? roomBed.room_id : 0
                    });
                }

                userBill.RoomTotal = userBill.Rooms.Sum(x => x.Nights * x.Price);
            }
            catch { }
        }

        private bool IsLessThanMinimumStay(List<UserBed> userBeds)
        {
            if (userBeds.Count == 1)
            {
                if ((userBeds.First().end_date.Value - userBeds.First().start_date).TotalHours <= 6)
                {
                    return true;
                }
            }
            return false;
        }

        private int GetCheckoutTime()
        {
            string checkOutTimeStr = ConfigurationManager.AppSettings["checkout-time"];
            int checkoutTime = 17;
            if (!string.IsNullOrEmpty(checkOutTimeStr))
            {
                checkoutTime = int.Parse(checkOutTimeStr);
            }
            return checkoutTime;
        }

        public List<Order> GetOrdersByMonth(List<int> userIds, DateTime date)
        {
            string startPeriod = new DateTime(date.Year, date.Month, 1).ToMySqlDateString();
            DateTime next = date.AddMonths(1);
            string endPeriod = new DateTime(next.Year, next.Month, 1).ToMySqlDateString();
            return genericRepository.Get<Order>(string.Format("select from menu_order where user_id in ({0}) and order_date > '{1}' and order_date < '{2}'", string.Join(",", userIds), startPeriod, endPeriod)).ToList();
        }

        private void AdjustNightsAfterRoomChange(RoomRow previousRoom, RoomRow newRoom)
        {
            //** Check if neccesary to substract a night from the previous or current room

            bool pr_IsAfterMidnight = previousRoom.EndDate.Value.Hour >= 0 && previousRoom.EndDate.Value.Hour < 12;
            bool pr_IsMoreThanOneNight = previousRoom.Nights > 1;
            bool pr_IsMoreThanMinimumHours = (previousRoom.EndDate.Value - previousRoom.StartDate).TotalHours > 6;

            bool nr_IsMoreThanMinimumHours = (newRoom.EndDate.Value - newRoom.StartDate).TotalHours > 6;

            if (pr_IsAfterMidnight)
            {
                //Previous Room is charged for the night
                if (pr_IsMoreThanOneNight || pr_IsMoreThanMinimumHours)
                {
                    double nr_NumberOfHoursUntilCheckOut = (new DateTime(newRoom.StartDate.Year, newRoom.StartDate.Month, newRoom.StartDate.Day, 14, 0, 0) - newRoom.StartDate).TotalHours;
                    //New Room is also charged for the night
                    if (nr_NumberOfHoursUntilCheckOut > 6)
                    {
                        //Substract from previous room
                        previousRoom.Nights--;
                    }
                    else
                    {
                        //Substract from new room
                        newRoom.Nights = newRoom.Nights > 0 ? (newRoom.Nights - 1) : 0;
                    }
                }

            }
            else
            {
                double pr_NumberOfHoursFromCheckinTime = (previousRoom.EndDate.Value - new DateTime(previousRoom.EndDate.Value.Year, previousRoom.EndDate.Value.Month, previousRoom.EndDate.Value.Day, 14, 0, 0)).TotalHours;
                //Previous Room is charged for the night
                if (previousRoom.EndDate.Value.Hour > 17)
                {
                    //Substract from new room
                    newRoom.Nights = newRoom.Nights > 0 ? (newRoom.Nights - 1) : 0;
                }
            }
        }

    }
}