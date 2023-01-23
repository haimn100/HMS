using casa_benjamin.Helpers;
using casa_benjamin.Modules.Shared.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    public class AnalyticsController : Controller
    {

        public ActionResult Index()
        {
            return View("~/Views/Admin/Analytics/Index.cshtml");
        }


        // GET: Graph
        public ActionResult OrdersByDateTime(GraphDateTimeRequest req)
        {
            var data = GetGraphDatePointsCount("menu_order", "order_date", req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult OrdersIncomeByDateTime(GraphDateTimeRequest req)
        {
            var data = GetGraphDatePointsSum("menu_order", "order_date", "total", req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckoutsByDateTime(GraphDateTimeRequest req)
        {
            var data = GetGraphDatePointsCount("check_outs", "check_out_date", req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult CheckinsByDateTime(GraphDateTimeRequest req)
        {
            var data = GetGraphDatePointsCount("user", "cidate", req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public static List<GraphDatePoint> GetGraphDatePointsCount(string table, string dateField, GraphDateTimeRequest req)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            string interval = string.Empty;

            SetDateAndInterval(req, out start, out end, out interval);

            string displayDate = dateField;
            if (req.interval == "week" || req.interval == "month")
            {
                displayDate = $"DATE_FORMAT({dateField}, \"%Y-%m-%d 00:00:00\")";
            }
            else if(req.interval == "year")
            {
                displayDate = $"DATE_FORMAT({dateField}, \"%Y-%m\")";
            }

            string q = $"select {displayDate} pointdate, count(*) pointval from {table} where Date({dateField}) >= '{start.ToMySqlDateString()}' and Date({dateField}) < '{end.ToMySqlDateString()}' group by {interval}({dateField})";
            var data = new GenericRepository().Get<GraphDatePoint>(q).ToList();         

            foreach (var item in data)
            {
                item.pointDate = DateTime.SpecifyKind(item.pointDate, DateTimeKind.Utc);
            }
            return data;
        }

        public static List<GraphDatePoint> GetGraphDatePointsSum(string table, string dateField,string sumField, GraphDateTimeRequest req)
        {
            DateTime start = DateTime.Now;
            DateTime end = DateTime.Now;
            string interval = string.Empty;

            SetDateAndInterval(req, out start, out end, out interval);

            string displayDate = dateField;
            if (req.interval == "week" || req.interval == "month")
            {
                displayDate = $"DATE_FORMAT({dateField}, \"%Y-%m-%d 00:00:00\")";
            }
            else if (req.interval == "year")
            {
                displayDate = $"DATE_FORMAT({dateField}, \"%Y-%m\")";
            }

            string q = $"select {displayDate} pointdate, sum({sumField}) pointval from {table} where Date({dateField}) >= '{start.ToMySqlDateString()}' and Date({dateField}) < '{end.ToMySqlDateString()}' group by {interval}({dateField})";
            var data = new GenericRepository().Get<GraphDatePoint>(q).ToList();

            foreach (var item in data)
            {
                item.pointDate = DateTime.SpecifyKind(item.pointDate, DateTimeKind.Utc);
            }
            return data;
        }

        public static void SetDateAndInterval(GraphDateTimeRequest req, out DateTime start, out DateTime end, out string interval)
        {
            start = new DateTime(req.year, req.month, req.day);
            end = start.AddDays(1);
            if (req.interval == "week")
            {
                start = start.AddDays(-7);
            }
            if (req.interval == "month")
            {
                start = new DateTime(req.year, req.month, 1);
                start = start.AddMonths(-1);
            }
            if (req.interval == "year")
            {
                start = new DateTime(req.year, req.month, 1);
                start = start.AddYears(-1);
            }

            interval = "hour";
            switch (req.interval)
            {
                case "day":
                    interval = "hour";
                    break;
                case "week":
                case "month":
                    interval = "day";
                    break;
                case "year":
                    interval = "month";
                    break;
            }
        }

        public class GraphDatePoint
        {
            public DateTime pointDate { get; set; }
            public object pointVal { get; set; }
        }

        public class GraphDateTimeRequest
        {
            public int year { get; set; }
            public int month { get; set; }
            public int day { get; set; }
            public string interval { get; set; }
        
        }
    }
}