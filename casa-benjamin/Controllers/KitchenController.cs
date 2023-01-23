using casa_benjamin.ActionFilters;
using casa_benjamin.Helpers;
using casa_benjamin.Models;
 
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.Kitchen.Services;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor")]
    public class KitchenController : Controller
    {
        // GET: Kitchen
        public ActionResult Index(int? userId)
        {
            ViewBag.UserId = userId;
            return View();
        }

        public ActionResult GuestOrders(int userId)
        {
            var userBill = new UIUserBill
            {
                User = UserManager.Instance.GetUser(userId),
                Orders = new List<OrderRow>(),
                Rooms = new List<RoomRow>()
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

                    User splitedBy = item.splited_by != userId ? userBill.User : UserManager.Instance.GetUser(item.splited_by);
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
            return View(userBill);
        }

        public ActionResult Orders(int? days)
        {
            int _days = 1;
            if (days.HasValue)
            {
                _days = days.Value;
            }

            ViewBag.days = _days;
            var orders = KitchenManager.Instance.GetOrdersByDays(_days).Where(x => !x.is_canceled).ToList();
            return View(orders);
        }

        public ActionResult Order(int orderId)
        {
            ViewBag.Order = KitchenManager.Instance.GetOrder(orderId);
            ViewBag.Items = KitchenManager.Instance.GetOrderItems(orderId);
            ViewBag.Discounts = UserManager.Instance.GetUserDiscountByOrderId(orderId);

            return View();
        }

        [HttpPost]
        public string SaveOrder(UIOrder order)
        {
            string result = string.Empty;
            int orderId = 0;
            try
            {
                if(order.items != null && order.items.Count > 0)
                {
                    orderId = KitchenManager.Instance.InsertOrder(order);
                }

                if(order.discounts != null && order.discounts.Count > 0)
                {
                    foreach (var item in order.discounts)
                    {
                        item.discount_date = DateTime.Now;
                        item.payment_type_id = order.pay_type_id;
                        item.staff = order.staffName;
                        
                        if(item.user_id == 0)
                        {
                            item.user_id = CacheManager.Instance.GhostUser.id;
                            item.user_name = CacheManager.Instance.GhostUser.name;
                        }
                        else
                        {
                            item.user_name = CacheManager.Instance.Users.First(x => x.id == item.user_id).name;
                        }

                        item.order_id = orderId;

                        UserManager.Instance.InsertUserDiscount(item);
                    }
                }

                Response.StatusCode = 200;                

                if(orderId > 0)
                {
                    result = orderId.ToString();
                }

                if(order.discounts != null && order.discounts.Count > 0)
                {
                    result += " with discount";
                }

                return result;
            }
            catch (Exception ex)
            {
                Response.StatusCode = 500;
                return ex.ToString();
            }
        }

        public ActionResult CancelOrder(int orderId, int staff, string redirectTo,string comment)
        {
            KitchenManager.Instance.CancelOrder(orderId, "", staff,comment);
            return Redirect(string.IsNullOrEmpty(redirectTo) ? "/" : redirectTo);
        }

        public ActionResult RemoveOrderItem(int orderId,int itemId,int staff)
        {
            var order = KitchenManager.Instance.GetOrder(orderId);
            var orderItem = KitchenManager.Instance.GetOrderItem(itemId);
            KitchenManager.Instance.RemoveOrderItem(itemId);

            //Update the order with the new price
            if(order.split_count > 0)
            {
                //If its a splited order we need to update all of them
                double newSplitTotal = order.split_total - orderItem.split_total;
                double newTotal = newSplitTotal / order.split_count;

                List<Order> ordersToUpdate = new List<Order>();
                List<Order> splitedOrders = KitchenManager.Instance.GetOrdersBySplitedOrderId(order.splited_order_id == 0 ? order.id:order.splited_order_id);
                Order mainOrder = KitchenManager.Instance.GetOrder(order.splited_order_id == 0 ? order.id : order.splited_order_id);

                mainOrder.split_total = newSplitTotal;
                mainOrder.total = newTotal;
                KitchenManager.Instance.UpdateOrder(mainOrder);

                foreach (var item in splitedOrders)
                {
                    item.split_total = newSplitTotal;
                    item.total = newTotal;
                    KitchenManager.Instance.UpdateOrder(item);
                }
            }
            else
            {
                order.total -= orderItem.total;
                KitchenManager.Instance.UpdateOrder(order);
            }
          
            Staff st = CacheManager.Instance.Staff.First(x => x.id == staff);
            UserManager.Instance.InsertStaffEvent(new StaffEvent
            {
                event_date = DateTime.Now,
                event_type_id = EventType.RemovedOrderItem,
                guest_id = order.user_id,
                staff_id = staff,
                staff_name = st.name,
                event_value = order.id.ToString()
            });

            var lastCREvent = CashRegisterManager.Instance.GetLastEventOrDefault();
            if (order.pay_type_id == PayType.Cash)
            {
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    event_type_id = EventType.RemovedOrderItem,
                    current_register_amount = lastCREvent.current_register_amount - (decimal)orderItem.total,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = order.id,
                    event_value = -(decimal)orderItem.total,
                    staff_id = st.id,
                    staff_name = st.name
                });
            }

            //try
            //{
            //    StockItem stockItem = StockManager.Instance.GetStockItemByMenuItemId(itemId);
            //    if (stockItem != null && stockItem.quantity > 0)
            //    {
            //        stockItem.quantity++;
            //        StockManager.Instance.UpdateStockItem(stockItem);
            //    }
            //}
            //catch (Exception ex)
            //{
            //    Trace.TraceError(ex.ToString());
            //}

            return Redirect(Request.UrlReferrer.AbsoluteUri);
        }

        public ActionResult MenuItems()
        {
            var data = new GenericRepository().Get<MenuItem>("select * from menu_item");
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Ingredients()
        {
            var data = new GenericRepository().Get<MenuItem>("select * from ingredient");
            data = data.OrderBy(x => x.name).ToList();
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult Categories()
        {
            var data = new GenericRepository().Get<MenuCategory>("select * from menu_category");
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult GetOrdersGraph(int year, int month,int day, string offset)
        {
            DateTime start = new DateTime(year, month, day);
            DateTime end = start.AddDays(1);
            if (offset == "week")
            {
                start = new DateTime(year, month, day);
                end = start.AddDays(7);
            }
            if (offset == "month")
            {
                start = new DateTime(year, month, 1);
                end = start.AddMonths(1);
            }
            if (offset == "year")
            {
                start = new DateTime(year, month, 1);
                end = start.AddYears(1);
            }

            string q = $"select * from menu_order where Date(order_date) >= '{start.ToMySqlDateString()}' and Date(order_date) < '{end.ToMySqlDateString()}'";
            var data = new GenericRepository().Get<Order>(q).ToList();
            object result = null;

            foreach (var item in data)
            {
               item.order_date = DateTime.SpecifyKind(item.order_date, DateTimeKind.Utc);
            }
            
            
            if(offset == "day")
            {
                result = data
                .GroupBy(x => x.order_date.Hour)
                .Select(grp => new { ds = grp.First().order_date.ToString(),  date = grp.First().order_date, hour = grp.Key, count = grp.Count() })
                .ToList();
            }
            if (offset == "month" || offset == "week")
            {
                result = data
               .GroupBy(x => x.order_date.Day)
               .Select(grp => new { ds = grp.First().order_date.ToString(), date = grp.First().order_date, hour = grp.Key, count = grp.Count() })
               .ToList();
            }
            if (offset == "year")
            {
                result = data
              .GroupBy(x => x.order_date.Month)
              .Select(grp => new { ds = grp.First().order_date.ToString(), date = grp.First().order_date, hour = grp.Key, count = grp.Count() })
              .ToList();
            }
            return new JsonResult() { Data = result, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}