using casa_benjamin.Managers;
using casa_benjamin.Models;
using System;
using System.IO;
using System.Web.Mvc;
using System.Linq;
using NLog;
using casa_benjamin.Internalization;
using casa_benjamin.Helpers;
using casa_benjamin.ActionFilters;
using casa_benjamin.Modules.Booking.Reservation.Controllers;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.User.Services;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.Kitchen.Services;
using casa_benjamin.Modules.Booking.Room.Enums;

namespace casa_benjamin.Controllers
{
    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor")]
    public class GuestController : Controller
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        // GET: Guest
        public ActionResult CheckIn(int bedId,int bedNumber)
        {
            bool isChrome = Request.Browser.Browser.ToLower() == "chrome" && !Request.UserAgent.ToLower().Contains("edge");
            if (isChrome && !Request.Url.AbsoluteUri.Contains("https"))
            {
                return Redirect(Request.Url.AbsoluteUri.Replace("http", "https"));
            }
            
            ViewBag.BedId = bedId;
            ViewBag.BedNumber = bedNumber;
            ViewBag.Room = CacheManager.Instance.GetRoomByBed(bedId);
            return View();
        }

        public ActionResult CheckOut(int userId,int staff, int totalCash, int totalCredit,decimal creditChargePercentage)
        {
            User user = UserManager.Instance.GetUser(userId);


            if (user.is_checked_out)
            {
                throw new Exception(I18n.T("User is already checked out!"));
            }            

            UserManager.Instance.CheckOutUser(userId,staff,totalCash,totalCredit, creditChargePercentage);

            if (user.res_id.HasValue)
            {
                try
                {
                    var res = ReservationManager.Instance.GetReservation(user.res_id.Value);
                    res.status = ReserVationStatus.CheckedOut;
                    ReservationManager.Instance.UpdateReservationStatus(res.res_id, ReserVationStatus.CheckedOut);
                    ReservationController.SendStatusChangedEmail(res, res.from_channel_manager ? "CM CheckOut" : "IH CheckOut");
                }
                catch { }
            }

            string message = I18n.T("Successfully checked out user") + " " + user.name;
            CacheManager.Instance.RefreshUsers();
            CacheManager.Instance.RefreshRooms();
            return Redirect("/?alert="+ message +"&alerttype=success");
        }         

        [HttpPost]
        public string CheckInForm(string image,string employee,string comment,int bed_price, User user)
        {
            try
            {
                User dbUser =  UserManager.Instance.GetUserByBed(user.bed_id);
                if(dbUser != null && !dbUser.is_checked_out)
                {
                    throw new Exception(I18n.T("Bed is already taken!"));
                }

                User existingUser = UserManager.Instance.GetActiveUser(user.passport);
                if(existingUser != null)
                {
                    throw new Exception(I18n.T("User already exist in bed ") + existingUser.bed_id);
                }

                if (user.cidate == DateTime.MinValue)
                {
                    throw new Exception("Check-In Date is not Valid");
                }

                Room room = CacheManager.Instance.GetRoomByBed(user.bed_id);
                int? reservationId = null;
                if (!user.res_id.HasValue)
                {
                    var res = new Reservation
                    {
                        res_id = Reservation.GenereateId(),
                        from_channel_manager = false,
                        number_of_people = 1,
                        res_date = user.cidate,
                        res_date_end = user.intended_codate,
                        res_name = user.name + " " + user.last_name,
                        room_id = room.id,
                        room_type = room.room_type_id,
                        sex = user.sex,
                        status = ReserVationStatus.CheckedIn
                    };
                    string error = ReservationManager.Instance.AddReservation(res);
                    if (!string.IsNullOrEmpty(error))
                    {
                        return error.Replace("?alert=","");
                    }
                    reservationId = res.res_id;
                    ReservationController.SendStatusChangedEmail(res, "IH CheckIn");
                }
                else
                {
                    var res = ReservationManager.Instance.GetReservation(user.res_id.Value);
                    if(res.status != ReserVationStatus.CheckedIn)
                    {

                        res.status = ReserVationStatus.CheckedIn;
                        ReservationManager.Instance.UpdateReservation(res);
                        ReservationController.SendStatusChangedEmail(res, res.from_channel_manager ? "CM CheckIn" : "IH CheckIn");
                    }
                    reservationId = user.res_id;
                }


                if (string.IsNullOrEmpty(image))
                {
                    user.pic = "no-image.jpg";
                }
                else
                {
                    string imageUrl = Server.MapPath("/GuestImages") + "\\" + user.passport + ".png";
                    using (FileStream fs = new FileStream(imageUrl, FileMode.OpenOrCreate, FileAccess.ReadWrite))
                    {
                        using (BinaryWriter bw = new BinaryWriter(fs))
                        {
                            byte[] data = Convert.FromBase64String(image);
                            bw.Write(data);
                            bw.Close();
                        }
                    }
                    user.pic = user.passport + ".png";
                }


                user.checked_in_by = employee;
                user.res_id = reservationId;
                UserManager.Instance.CheckInUser(user, bed_price, comment);            
                CacheManager.Instance.RefreshUsers();
                return "ok";
            }
            catch(Exception ex)
            {
                return ex.ToString();
            }      
        }

        [HttpPost]
        public ActionResult MoveUserBed(int userId,int destBed,int destPrice,string comment,int staffId)
        {
            var user = CacheManager.Instance.Users.First(x => x.id == userId);
            var room = CacheManager.Instance.GetRoomByBed(destBed);

            var userCurrentRoom = UserManager.Instance.GetRoom(CacheManager.Instance.GetRoomByBed(user.bed_id).id);
            var userCurrentBedId = user.bed_id;

            if (UserManager.Instance.GetUserByBed(destBed) != null)
            {
                string error = I18n.T("Bed is already taken");
                string query = string.IsNullOrEmpty(Request.UrlReferrer.Query) ? "?" : Request.UrlReferrer.Query + "&";
                query += "alert=" + error + "&alerttype=error";
                return Redirect(Request.UrlReferrer.AbsolutePath + query);
            }

            if (user.res_id.HasValue)
            {
                Reservation res = ReservationManager.Instance.GetReservation(user.res_id.Value);
                if (res != null)
                {

                    string result = ReservationManager.Instance.CheckReservationAvailability(room.room_number, DateTime.Now, user.intended_codate, 1, res.res_id);
                    if (result != "ok")
                    {
                        string error = result;
                        string query = string.IsNullOrEmpty(Request.UrlReferrer.Query) ? "?" : Request.UrlReferrer.Query + "&";
                        query += "alert=" + error + "&alerttype=error";
                        return Redirect(Request.UrlReferrer.AbsolutePath + query);
                    }
                    else
                    {
                        res.room_id = room.room_number;
                        res.room_type = room.room_type_id;
                        ReservationManager.Instance.UpdateReservationRoom(res.res_id, res.room_id, (int)res.room_type);
                    }
                }
            }

            UserManager.Instance.MoveGuest(user.id, destBed, destPrice, comment);

            try
            {
                var genericRepository = new GenericRepository();

                if (!userCurrentRoom.is_clean_required && !userCurrentRoom.is_cleaning_inspection_required)
                {
                    userCurrentRoom.is_cleaning_inspection_required = true;
                    userCurrentRoom.is_cleaning_inspection_date = DateTime.Now;
                    genericRepository.Update(userCurrentRoom);

                    genericRepository.ExecuteScalar($"update room_bed set last_checkout = '{DateTime.Now.ToMySqlDateTimeString()}' where room_id = {userCurrentRoom.id} and bed_id = {userCurrentBedId}");
                }
                CacheManager.Instance.RefreshRooms();
            }
            catch { }


            Staff st = CacheManager.Instance.Staff.First(x => x.id == staffId);
            UserManager.Instance.InsertStaffEvent(new StaffEvent
            {
                event_date = DateTime.Now,
                event_type_id = EventType.MovedBed,
                staff_id = staffId,
                staff_name = st.name,
                guest_id = user.id,
                event_value = user.bed_id + " > " + destBed 
            });

            string alert = I18n.T("Successfully Moved {{0}} To Bed {{1}}",user.name,destBed.ToString());
            return Redirect("/?alert=" + alert + "&alerttype=success");
        }

        [HttpPost]
        public ActionResult UpdateProfile(User user,int pricePerNight)
        {
            User _user = UserManager.Instance.GetUser(user.id);
            _user.nationality = user.nationality;
            _user.email = user.email;
            _user.name = user.name;
            _user.passport = user.passport;
            _user.phone = user.phone;
            _user.sex = user.sex;
            _user.birth_date = user.birth_date;
            _user.arrival = user.arrival;
            _user.destination = user.destination;
            _user.profession = user.profession;
            _user.last_name = user.last_name;

            if(_user.intended_codate != user.intended_codate)
            {
                if (_user.res_id.HasValue)
                {
                    var res = ReservationManager.Instance.GetReservation(_user.res_id.Value);
                    string result = null;
                    if (res.room_type == RoomType.Private)
                    {
                        result = ReservationManager.Instance.CheckReservationAvailability(res.room_id, _user.intended_codate, user.intended_codate, 1,res.res_id);
                    }
                    else
                    {
                        result = ReservationManager.Instance.CheckReservationAvailability(res.room_id, _user.intended_codate, user.intended_codate, 1);
                    }

                    if(result != "ok")
                    {
                        return Redirect(Request.UrlReferrer.AbsoluteUri + result.Replace("?","&"));
                    }

                    ReservationManager.Instance.ExtendReservation(res, user.intended_codate);
                    ReservationController.SendStatusChangedEmail(res, (res.from_channel_manager ? "CM":"IH") +  " Res Extended");
                }
            }

            _user.intended_codate = user.intended_codate;

            //if (_user.cidate != user.cidate)
            //{
            //    _user.cidate = user.cidate;
            //    var firstUserBed = UserManager.Instance.GetUserBeds(user.id).First();
            //    firstUserBed.start_date = user.cidate;
            //    new GenericRepository().Update(firstUserBed);
            //}

            UserBed userLastBed = UserManager.Instance.GetUserBeds(user.id).OrderBy(x => x.id).Last();
            if(userLastBed.price != pricePerNight)
            {
                new GenericRepository().Insert(new UserBed
                {
                    start_date = DateTime.Now,
                    comment = "Changed night price",
                    bed_id = userLastBed.bed_id,
                    price = pricePerNight,
                    user_id = user.id
                });
                userLastBed.end_date = DateTime.Now;
                UserManager.Instance.UpdateUserBed(userLastBed);
            }

            UserManager.Instance.UpdateUser(_user);
            CacheManager.Instance.RefreshUsers();

            if(_user.sex != user.sex)
            {
                CacheManager.Instance.RefreshRooms();
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult Bill(int userId)
        {
            UIUserBill userBill = UserManager.Instance.GetUserBill(userId);
            if (userBill.User.is_checked_out)
            {
                CheckOut co = UserManager.Instance.GetCheckOut(userBill.User.id);
                /* Total can be different because the calculation of credit card commision is
                   is done after GetUserBill() when the user checks out and then alter the total and credit_total and 
                   save it in the checkouts table
                 */
                userBill.Total = co.total;
                ViewBag.CheckOut = co;
            }

            return View(userBill);
        }

        public ActionResult UserProfile(int userId)
        {
            User user = UserManager.Instance.GetUser(userId);
            UserBed userLastBed = UserManager.Instance.GetUserBeds(userId).OrderBy(x => x.id).Last();
            ViewBag.UserLastBed = userLastBed;
            UIUserBill bill = UserManager.Instance.GetUserBill(user.id);
            ViewBag.Bill = bill;
            return View(user);
        }

        public ActionResult GetUserByPassport(string passport)
        {           
            return Json(UserManager.Instance.GetUserByPassport(passport), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetSplitedOrderUser(int orderId)
        {
            return Json(KitchenManager.Instance.GetSplitedOrderUsers(orderId),JsonRequestBehavior.AllowGet);
        }
        
        public ActionResult UpdateCheckoutPayment(int userid, int totalCash, int totalCredit , int staff)
        {
            CheckOut co = UserManager.Instance.GetCheckOut(userid);
            Staff _staff = UserManager.Instance.GetStaff(staff);

            //UPDATE CASH REGISTER
            var lastCashRegisterEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
            if (totalCash != co.total_cash) 
            {
                double cashDiffFFromPreviousCheckout = totalCash - co.total_cash;
                if(cashDiffFFromPreviousCheckout > 0)
                {
                    CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                    {
                        current_register_amount = lastCashRegisterEvent.current_register_amount + (decimal)cashDiffFFromPreviousCheckout,
                        event_date = DateTimeHelper.GetCurrentDateTime(),
                        event_realted_entity_id = userid,
                        event_type_id = EventType.CashRegisterAddFromCheckOutUpdate,
                        staff_id = _staff.id,
                        staff_name = _staff.name,
                        event_value = (decimal)cashDiffFFromPreviousCheckout,
                        comment = I18n.T("Updated Cash From CheckOut: {{0}} > {{1}}", co.total_cash.ToString(), totalCash.ToString())
                    });
                }               
            }

            if (totalCredit != co.total_credit)
            {
                double creditDiffFFromPreviousCheckout = totalCredit - co.total_credit;
                double creditCardCharge = UserManager.Instance.GetCreditCardCharge(creditDiffFFromPreviousCheckout, co.credit_charge_percentage,0);

                if (CacheManager.Instance.AppSettings.AddCreditCardChargeToRegister && creditCardCharge > 0)
                {
                    CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                    {
                        current_register_amount = lastCashRegisterEvent.current_register_amount + (decimal)creditCardCharge,
                        event_date = DateTimeHelper.GetCurrentDateTime(),
                        event_realted_entity_id = userid,
                        event_type_id = EventType.CashRegisterAddFromCheckOutUpdate,
                        staff_id = _staff.id,
                        staff_name = _staff.name,
                        event_value = (decimal)creditCardCharge
                    });
                }
                else
                {
                    totalCredit += Convert.ToInt32(creditCardCharge);
                }              
            }

            co.total_cash = totalCash;
            co.total_credit = totalCredit;
            UserManager.Instance.UpdateCheckOut(co);
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult GetGuestByBarcode(string barcode)
        {
            User user = UserManager.Instance.GetUserByBarcode(barcode);
            if(user != null)
            {
                return Json(user, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { error = "User Not Found" }, JsonRequestBehavior.AllowGet);
            }
        }

        public ActionResult GetReservationsInDates(DateTime from, DateTime to,int bedId)
        {
            int roomId = CacheManager.Instance.Rooms.First(x => x.beds.Exists(b => { return b.bed_id == bedId; })).room.id; 
            return Json(UserManager.Instance.GetReservationsInDates(from, to, roomId),JsonRequestBehavior.AllowGet);
        }

        public void SetResident(int userId,bool state)
        {
            if (state)
            {
                UserManager.Instance.AddResident(userId);
            }
            else
            {
                UserManager.Instance.RemoveResident(userId);
            }
            CacheManager.Instance.RefreshUsers();
        }

        public void AddUserDiscount(UserDiscount userDiscount)
        {
            UserManager.Instance.InsertUserDiscount(userDiscount);
        }

        public ActionResult AddUserDiscountForBill (int user_id,int staffid,int discount_amount, string discount_comment)
        {
            var staff = CacheManager.Instance.Staff.FirstOrDefault(x => x.id == staffid);
            UserManager.Instance.InsertUserDiscount(new UserDiscount
            {
                comment = discount_comment,
                discount_date = DateTime.Now,
                payment_type_id = (PayType)1,
                price = discount_amount,
                user_id = user_id,
                staff = staff == null ? "":staff.name
            });
            return Redirect(HttpContext.Request.UrlReferrer.AbsoluteUri);

        }

        public void RemoveUserDiscount(int id)
        {
            var d = UserManager.Instance.GetUserDiscountByDiscountId(id);
            UserManager.Instance.DeleteUserDiscount(d);
        }
        
        public void UpdateUserCheckInDate(DateTime date, int userId)
        {
            var user = UserManager.Instance.GetUser(userId);
            user.cidate = date;
            UserManager.Instance.UpdateUser(user);
            var firstUserBed = UserManager.Instance.GetUserBeds(userId).First();
            firstUserBed.start_date = date;
            new GenericRepository().Update(firstUserBed);

        }
        
        public ActionResult Delete(int id)
        {
            UserManager.Instance.DeleteUser(id);
            CacheManager.Instance.Refresh();
            return Redirect("/?alert=Guest was deleted&alerttype=success");
        }

        [HttpPost]
        public ActionResult UpdatePrePayment(UserPrePay model, int staffId)
        {
            GenericRepository genericRepository = new GenericRepository();
            UserPrePay prepay = UserManager.Instance.GetUserPrePayById(model.id);
            Staff staff = UserManager.Instance.GetStaff(staffId);
            CashRegisterEvent lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();

            int oldAmount = prepay.amount;
            int newAmount = model.amount;
                
            prepay.amount = model.amount;
            prepay.comment = model.comment;
            prepay.staff = staff.name;
            prepay.prepay_date = DateTimeHelper.GetCurrentDateTime();
            genericRepository.Update(prepay);

            //**Not allowing to update pay type to avoid complexity
            if (prepay.pay_type == PayType.Cash)
            {
                int diffAmount = newAmount - oldAmount;
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastEvent.current_register_amount + diffAmount,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_type_id = EventType.CashRegisterUpdatePrePayment,
                    event_realted_entity_id = model.user_id,
                    event_value = diffAmount,
                    staff_id = staff.id,
                    staff_name = staff.name
                });
            }

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        [HttpPost]
        public ActionResult AddPrePayment(UserPrePay model, int staffId)
        {
            GenericRepository genericRepository = new GenericRepository();
            Staff staff = UserManager.Instance.GetStaff(staffId);

            UserPrePay data = new UserPrePay
            {
                user_id = model.user_id,
                amount = model.amount, 
                comment = model.comment,
                pay_type = model.pay_type,
                staff = staff.name,
                deposit_credit_card_charge = model.deposit_credit_card_charge,
                prepay_date = DateTimeHelper.GetCurrentDateTime()
            };
            genericRepository.Insert(data);

            if (model.pay_type == PayType.Cash)
            {
                CashRegisterEvent lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastEvent.current_register_amount + model.amount,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_type_id = EventType.CashRegisterAddPrePayment,
                    event_realted_entity_id = model.user_id,
                    event_value = model.amount,
                    staff_id = staff.id,
                    staff_name = staff.name
                });
            }
            

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public void RemovePrePayment(int id,int staffId)
        {
            Staff staff = UserManager.Instance.GetStaff(staffId);
            GenericRepository genericRepository = new GenericRepository();
            UserPrePay prepay = UserManager.Instance.GetUserPrePayById(id);
            genericRepository.Delete(prepay);

            if(prepay.pay_type == PayType.Cash)
            {
                CashRegisterEvent lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastEvent.current_register_amount - prepay.amount,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_type_id = EventType.CashRegisterRemovePrePayment,
                    event_realted_entity_id = prepay.user_id,
                    event_value = prepay.amount,
                    staff_id = staff.id,
                    staff_name = staff.name
                });
            }
        }


    }
}