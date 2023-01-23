using casa_benjamin.Helpers;
using casa_benjamin.Managers;
using casa_benjamin.Models;
using casa_benjamin.Modules.Booking.Room.Services;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Shared.Values;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web.Hosting;
using System.Web.Mvc;

namespace casa_benjamin.Modules.Booking.Reservation.Controllers
{

    public class ReservationController : Controller
    {
        private readonly RoomService roomService = new RoomService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);

        // GET: Reservation
        public ActionResult Index()
        {
            return View("~/Views/Booking/Reservation/Index.cshtml");
        }

        [HttpGet]
        public ActionResult AllItemsTable(PagedTableRequest req)
        {
            var data = ReservationManager.Instance.GetReservationsTable(req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult OneItemTable(int reservationId)
        {
            var data = ReservationManager.Instance.GetReservationAllDates(reservationId).First();
            return new JsonResult()
            {
                Data = new PagedTableResponse<Models.Reservation>
                {
                    data = new List<Models.Reservation> { data }
                },
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult List(DateTime? from = null, DateTime? to = null)
        {
            DateTime _from = from ?? DateTime.Now.AddDays(-7);
            DateTime _to = to ?? DateTime.Now;
            var data = ReservationManager.Instance.GetReservationsList(_from, _to);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult FindOne(int id, int roomId)
        {
            var data = ReservationManager.Instance.FindOne(id, roomId);
            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult FindMany(string ids, int roomId)
        {
            List<Models.Reservation> result = new List<Models.Reservation>();
            var reservations = ReservationManager.Instance.FindMany(ids, roomId).GroupBy(x => x.res_id);
            foreach (var resGroup in reservations)
            {
                result.Add(resGroup.OrderBy(x => x.res_date).First());
            }
            return new JsonResult { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult Edit(Models.Reservation reservation)
        {
            try
            {
                ReservationManager.Instance.UpdateReservation(reservation);

                PropogateReservationsToSystem();
                return Redirect(Request.UrlReferrer.AbsoluteUri);
            }catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }
        }

        public ActionResult Remove(Models.Reservation res)
        {
            ReservationManager.Instance.DeleteReservation(res.res_id);
            PropogateReservationsToSystem();

            return Redirect(Request.UrlReferrer.AbsoluteUri);           
        }

        [HttpPost]
        public ActionResult Add(Models.Reservation reservation)
        {
            reservation.status = ReserVationStatus.Confirmed;
            reservation.res_id = Models.Reservation.GenereateId();
            
            //set redundent legacy room type field
            reservation.room_type = roomService.FindOne(reservation.room_id).room_type_id;
            
            try
            {
                string error = ReservationManager.Instance.AddReservation(reservation);

                if (!string.IsNullOrEmpty(error))
                {
                    return new HttpStatusCodeResult(HttpStatusCode.BadRequest, error);
                }
            }catch(Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest, ex.Message);
            }


            PropogateReservationsToSystem();
            if (!string.IsNullOrWhiteSpace(reservation.origin))
            {
                reservation.from_channel_manager = true;
                try
                {
                    SendStatusChangedEmail(reservation, "IH Reservation");
                }
                catch { }
            }
            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        private void PropogateReservationsToSystem()
        {
            CacheManager.Instance.RefreshRooms();
        }

        public ActionResult Calendar(ReportDateQuery dates)
        {
            dates.To = dates.To.HasValue ? dates.To.Value : DateTimeHelper.GetCurrentDateTime().AddMonths(1);
            dates.From = dates.From.HasValue ? dates.From.Value : DateTimeHelper.GetCurrentDateTime();
            var model = ReservationManager.Instance.GetReservationCalendar(dates.From.Value, dates.To.Value);
            ViewBag.Dates = dates;
            return View("~/Views/Booking/Reservation/Calendar.cshtml", model);
        }

        public ActionResult GetReservation(int id)
        {
            return Json(ReservationManager.Instance.GetReservation(id), JsonRequestBehavior.AllowGet);
        }

        public ActionResult SendConfirmation(int resID)
        {
            try
            {
                var res = ReservationManager.Instance.GetReservation(resID);
                string recipient = res.res_email;
                if (string.IsNullOrEmpty(recipient)) throw new Exception();
                SendStatusConifrmationEmail(res);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(HttpStatusCode.InternalServerError, ex.Message);
            }

            return new HttpStatusCodeResult(HttpStatusCode.OK);

        }



        public static void SendMail(string title, string body, string from, string to)
        {
            string[] smtp = ConfigurationManager.AppSettings["smtpclient"].Split(':');

            var client = new SmtpClient("smtp.gmail.com", 587)
            {
                Credentials = new NetworkCredential(smtp[0], smtp[1]),
                EnableSsl = true
            };

            MailMessage msg = new MailMessage(from, to, title, body);
            msg.IsBodyHtml = true;          
            client.Send(msg);

        }

        public static void SendStatusChangedEmail(Models.Reservation res, string type)
        {
            try
            {
                string[] emailContent = GetStatusChangeEmailContent(res, type);
                SendMail(emailContent[0], emailContent[1], "system@weplaya",
                    ConfigurationManager.AppSettings["email"].Split(':')[0]);
            }
            catch { }
        }

        public static void SendStatusConifrmationEmail(Models.Reservation res)
        {
            string to = res.res_email;
            string from = "team@weplaya";

            string title = $"Confirmed booking from WePlaya Hostel";
            string fileContents = System.IO.File.ReadAllText(HostingEnvironment.MapPath(@"~/App_Data/ConfirmationMail.html"));
            string body = fileContents.Replace("{res_name}", res.res_name)
                                        .Replace("{res_id}", res.id.ToString())
                                      .Replace("{checkin}", res.res_date.ToShortUIDateString())
                                      .Replace("{checkout}", res.res_date_end.ToShortUIDateString())
                                      .Replace("{room}", res.room_id.ToString())
                                      .Replace("{guests}", res.number_of_people.ToString());
            SendMail(title, body, from, to);
        }

        public static string[] GetStatusChangeEmailContent(Models.Reservation res, string type)
        {
            string title = $"{type}: {res.res_name} | {res.res_date.ToShortDateString()} - {res.res_date_end.ToShortDateString()} | room {res.room_id} | nights {res.nights} | people {res.number_of_people}";
            string body = $"<table>" +
                $"<tr><td>Name</td><td>{res.res_name}</td></tr>" +
                $"<tr><td>Check In</td><td>{res.res_date.ToShortDateString()}</td></tr>" +
                $"<tr><td>Check Out</td><td>{res.res_date_end.ToShortDateString()}</td></tr>" +
                $"<tr><td>Room</td><td>{res.room_id}</td></tr>" +
                $"<tr><td>Nights</td><td>{res.nights}</td></tr>" +
                $"<tr><td>Number of People</td><td>{res.number_of_people}</td></tr>" +
                $"</table>";

            return new string[] { title, body };
        }
    }
}