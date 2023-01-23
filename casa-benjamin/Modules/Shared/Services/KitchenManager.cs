using casa_benjamin.Data;
using casa_benjamin.Models;
 
using System;
using System.Collections.Generic;
using System.Linq;
using System.Diagnostics;
using System.Configuration;
using Dapper;
using casa_benjamin.Helpers;
using casa_benjamin.Modules.Restaurant.Inventory.Services;
using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using casa_benjamin.Modules.User.Services;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Order.Entities;
using casa_benjamin.Modules.CashRegister.Services;
using casa_benjamin.Modules.Shared.Enums;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.CashRegister.Entities;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Restaurant.Inventory.Values;

namespace casa_benjamin.Modules.Kitchen.Services
{
    public sealed class KitchenManager
    {
        private static volatile KitchenManager instance;
        private static object syncRoot = new Object();

        private KitchenRepository kitchenRepository;
        private GenericRepository genericRepository;
        private ProductService productService;

        private KitchenManager()
        {
            kitchenRepository = new KitchenRepository();
            genericRepository = new GenericRepository();
            productService = new ProductService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
        }

        public static KitchenManager Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (syncRoot)
                    {
                        if (instance == null)
                            instance = new KitchenManager();
                    }
                }

                return instance;
            }
        }

        public long InsertMenuCategory(MenuCategory item)
        {
            return genericRepository.Insert(item);
        }

        public long InsertMenuItem(MenuItem item)
        {
            return genericRepository.Insert(item);
        }

        public long InsertIngredient(Ingredient item)
        {
            return genericRepository.Insert(item);
        }

        public bool DeleteIngredient(Ingredient item)
        {
            return genericRepository.Delete(item);
        }

        public List<MenuItemIngredient> GetMenuItemIngredientsByIngredientId(int id)
        {
            return genericRepository.Get<MenuItemIngredient>("select * from menu_item_ingredient where ingredient_id = " + id).ToList();
        }

        public List<MenuItemIngredient> GetMenuItemIngredientsByMenuItemId(int id)
        {
            return genericRepository.Get<MenuItemIngredient>("select * from menu_item_ingredient where menu_item_id = " + id).ToList();
        }

        public MenuItemIngredient GetMenuItemIngredient(int id)
        {
            return genericRepository.Get<MenuItemIngredient>("select * from menu_item_ingredient where id = " + id).First();
        }

        public List<MenuItemIngredient> GetMenuItemIngredient(int menuItemId, List<int> ids)
        {
            return genericRepository.Get<MenuItemIngredient>($"select * from menu_item_ingredient where menu_item_id = {menuItemId} and ingredient_id in ({string.Join(",",ids)})");
        }

        public bool UpdateIngredient(Ingredient item)
        {
            return genericRepository.Update(item);
        }

        public bool UpdateOrder(Order item)
        {
            return genericRepository.Update(item);
        }

        public long InsertMenuItemIngredient(MenuItemIngredient item)
        {
            using (MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Execute("update menu_item_ingredient set ingredients_group = @group,ingredients_group_single_select = @sel where ingredients_group_number = @grpNumber and menu_item_id = @menuItemId",
                    new { group = item.ingredients_group,
                          sel = item.ingredients_group_single_select,
                          grpNumber = item.ingredients_group_number,
                          menuItemId = item.menu_item_id });
            }
            return genericRepository.Insert(item);
        }

        public bool UpdateMenuItemIngredient(MenuItemIngredient item)
        {
            return genericRepository.Update(item);
        }

        public void UpdateMenuItemIngredientGroup(string groupName, bool isSingleSelect, int menuItemId, int groupNumber)
        {
            using (MySql.Data.MySqlClient.MySqlConnection con = new MySql.Data.MySqlClient.MySqlConnection(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString))
            {
                con.Execute("update menu_item_ingredient set ingredients_group = @group,ingredients_group_single_select = @sel where ingredients_group_number = @grpNumber and menu_item_id = @menuItemId",
                    new { group = groupName,
                          sel = isSingleSelect,
                          grpNumber = groupNumber,
                          menuItemId = menuItemId
                    });
            }
        }

        public bool DeleteMenuItemIngredient(MenuItemIngredient item)
        {
            return genericRepository.Delete(item);
        }

        public int InsertOrder(UIOrder uiOrder)
        {
            Order order = new Order();
            var now = DateTimeHelper.GetCurrentDateTime();
            int orderId = 0;
            Staff.Entities.Staff staff = UserManager.Instance.GetStaff(uiOrder.staffId);

            foreach (var orderItem in uiOrder.items)
            {
                orderItem.order_date = now;
            }

            if (uiOrder.isCashUser)
            {
                order.user_id = CacheManager.Instance.GhostUser.id;
                order.user_name = uiOrder.userName = CacheManager.Instance.GhostUser.name;
            }
            else
            {
                order.user_id = uiOrder.userId;
                order.user_bed = uiOrder.userBed;
                order.user_name = uiOrder.userName;
            }

            if(uiOrder.splitUsers != null && uiOrder.splitUsers.Count > 0)
            {
                double splitedPrice = Math.Round(uiOrder.total / (uiOrder.splitUsers.Count + 1));
                order.split_count = uiOrder.splitUsers.Count + 1;
                order.split_total = uiOrder.total;
                order.total = splitedPrice;

                if (uiOrder.isCashUser)
                {
                    /* Because if the user is SYSTEM we will not create multiple orders with the splited bill
                     * That screw's up when we try to sum by split_total instead of total
                     */
                    order.split_total = 0;
                }

                order.splited_by = uiOrder.userId;
                order.staff_name = uiOrder.staffName;
                order.staff_id = uiOrder.staffId;
                order.order_date = DateTime.Now;
                order.pay_type_id = uiOrder.pay_type_id;
                order.menu_category_type = uiOrder.menu_category_type;
                order.credit_charge_percentage = uiOrder.creditCardChargePercentage;

                foreach (var orderItem in uiOrder.items)
                {
                    orderItem.split_total = orderItem.total;
                    orderItem.total = orderItem.total / (uiOrder.splitUsers.Count + 1);
                }

                order.discount = uiOrder.discounts == null ? 0 : uiOrder.discounts.Sum(x => x.price);
                AddCreditCardChargeIfNeeded(order, uiOrder);
                orderId = kitchenRepository.InsertOrder(order, uiOrder.items);

                if (!uiOrder.isCashUser)
                {
                    foreach (var guest in uiOrder.splitUsers)
                    {
                        Order splitedOrder = order.ShallowCopy();
                        splitedOrder.user_id = guest.id;
                        splitedOrder.splited_by = uiOrder.userId;
                        splitedOrder.splited_order_id = orderId;
                        splitedOrder.user_bed = guest.bed_id;
                        splitedOrder.user_name = guest.name;
                        splitedOrder.comment = "Splited with " + order.user_name + " order #" + orderId;
                        kitchenRepository.InsertOrder(splitedOrder, new List<OrderItems>());
                    }
                }
            }
            else
            {
                order.staff_name = uiOrder.staffName;
                order.staff_id = uiOrder.staffId;
                order.order_date = DateTime.Now;
                order.pay_type_id = uiOrder.pay_type_id;
                order.total = uiOrder.total;
                order.menu_category_type = uiOrder.menu_category_type;
                order.credit_charge_percentage = uiOrder.creditCardChargePercentage;

                if(uiOrder.splitCashCount > 0)
                {
                    order.split_count = uiOrder.splitCashCount + 1;
                    order.split_total = Math.Round(uiOrder.total / order.split_count);
                }

                order.discount = uiOrder.discounts == null ? 0 : uiOrder.discounts.Sum(x => x.price);

                AddCreditCardChargeIfNeeded(order, uiOrder);
                orderId = kitchenRepository.InsertOrder(order, uiOrder.items);
            }


            AddOrderCashRegisterEvents(order, orderId, uiOrder, staff);
            
            try
            {
                //Update inventory
                foreach (var orderItem in uiOrder.items)
                {
                    UpdateProductsInvetory(orderItem);                 
                }
            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }

            return orderId;
        }

        public void CancelOrder(int orderId,string reason,int staffId, string comment)
        {

            //Cancel the order
            Order order = genericRepository.Get<Order>("select * from menu_order where id = " + orderId).First();
            order.is_canceled = true;
            order.canceled_by_staff_id = staffId;
            order.comment = comment;
            genericRepository.Update(order);


            //Check if there are "connected" orders via Split bill
            List<Order> splitedOrders = genericRepository.Get<Order>("select * from menu_order where splited_order_id = " + orderId).ToList();
            foreach (var splitOrder in splitedOrders)
            {
                splitOrder.is_canceled = true;
                genericRepository.Update(splitOrder);
            }

            Staff.Entities.Staff st = CacheManager.Instance.Staff.First(x => x.id == staffId);
            genericRepository.Insert(new StaffEvent
            {
                staff_id = staffId,
                staff_name = st.name,
                event_type_id = EventType.CanceledOrder,
                event_value = orderId.ToString(),
                event_date = DateTime.Now
            });


            //Update cash register
            var lastCREvent = CashRegisterManager.Instance.GetLastEventOrDefault();
            if (order.pay_type_id == PayType.Cash)
            {
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    event_type_id = EventType.CanceledOrder,
                    current_register_amount = lastCREvent.current_register_amount - (decimal)order.total,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = order.id,
                    event_value = -(decimal)order.total,
                    staff_id = st.id,
                    staff_name = st.name
                });
            }
            /* If we add the credit extra charge to the cash register we 
               eed to remove it from the cash register */
            else if (order.pay_type_id == PayType.Credit 
                    && CacheManager.Instance.AppSettings.AddCreditCardChargeToRegister
                    && order.credit_charge_percentage > 0)
            {
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    event_type_id = EventType.CanceledOrder,
                    current_register_amount = lastCREvent.current_register_amount - order.credit_charge,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = order.id,
                    event_value = -order.credit_charge,
                    staff_id = st.id,
                    staff_name = st.name
                });
            }

            try
            {
                List<OrderItems> orderItems = GetOrderItems(orderId);
                //Update stock data
                //foreach (var item in orderItems)
                //{
                //    int menuItemId = item.menu_item_id;
                //    StockItem stockItem = StockManager.Instance.GetStockItemByMenuItemId(menuItemId);
                //    if (stockItem != null && stockItem.quantity > 0)
                //    {
                //        stockItem.quantity++;
                //        StockManager.Instance.UpdateStockItem(stockItem);
                //    }
                //}

            }
            catch (Exception ex)
            {
                Trace.TraceError(ex.ToString());
            }
        }

        public void UpdateCategory(MenuCategory cat)
        {
            genericRepository.Update(cat);
        }

        public MenuCategory GetMenuCategory(int id)
        {
            return genericRepository.Get<MenuCategory>("select * from menu_category where id = " + id).First();
        }

        public void UpdateMenuItem(MenuItem mi)
        {
            genericRepository.Update(mi);
        }

        public List<Order> GetOrders(int userId)
        {
            return genericRepository.Get<Order>("select * from menu_order where user_id = " + userId).ToList();
        }

        public Order GetOrder(int orderId)
        {
            return genericRepository.Get<Order>("select * from menu_order where id = " + orderId).First();
        }

        public List<OrderItems> GetOrderItems(int orderId)
        {
            return genericRepository.Get<OrderItems>("select * from order_items where order_id = " + orderId).ToList();
        }
      
        public List<Order> GetOrdersBySplitedOrderId(int splitedOrderId)
        {
            return genericRepository.Get<Order>("select * from menu_order where splited_order_id = " + splitedOrderId).ToList();
        }

        public OrderItems GetOrderItem(int itemId)
        {
            return genericRepository.Get<OrderItems>("select * from order_items where id = " + itemId).First();
        }

        public List<OrderRow> GetOrderItems(List<int> ordersIds)
        {
            if(ordersIds.Count == 0)
            {
                return new List<OrderRow>();
            }

            string ordersIdsStr = string.Join(",",ordersIds);
            List<Order> orders = genericRepository.Get<Order>("select * from menu_order where id in (" + ordersIdsStr + ")").ToList();          
            List<OrderItems> orderItems = genericRepository.Get<OrderItems>("select * from order_items where order_id in (" + ordersIdsStr + ")").ToList();
            List<OrderRow> orderRows = new List<OrderRow>();
            List<User.Entities.User> users = UserManager.Instance.GetUsers(orders.Select(x => x.user_id).ToList());
            users.Add(CacheManager.Instance.GhostUser);

            foreach (var item in orders)
            {
                orderRows.Add(new OrderRow
                {
                    Order = item,
                    OrderItems = orderItems.Where(x=> x.order_id == item.id).ToList(),
                    SplitedByUserId = item.splited_by,
                    SplitedByUserName = users.First(x=> x.id == item.user_id).name 
                });
            }

            return orderRows;
        }

        public List<OrderItems> GetOrderItems(int minOrder, int maxOrder)
        {
            return genericRepository.Get<OrderItems>("select * from order_items where order_id >= " + minOrder+ " and order_id <= " + maxOrder).ToList();
        }

        public List<Order> GetOrdersByDays(int days)
        {
            var now = DateTime.Now.AddDays(1).ToMySqlDateString();
            var till = DateTime.Now.AddDays(-days).ToMySqlDateString();
            return genericRepository.Get<Order>(string.Format("select * from menu_order where order_date >= '{0}' and order_date <= '{1}'",till,now)).ToList();
        }

        public List<User.Entities.User> GetSplitedOrderUsers(int orderId)
        {
            var orders = genericRepository.Get<Order>("select * from menu_order where splited_order_id =" + orderId).ToList();
            List<int> usersIds = orders.Select(x => x.user_id).ToList();
            List<User.Entities.User> users = genericRepository.Get<User.Entities.User>("select * from user where id in (" + string.Join(",", usersIds) + ")").ToList();
            return users;
        }

        public void RemoveOrderItem(int itemId)
        {
            var item = genericRepository.Get<OrderItems>("select * from order_items where id = " + itemId).First();
            genericRepository.Delete(item);
        }

        public List<MenuItem> GetMenuItems(MenuCategoryType catType)
        {
            return genericRepository.Get<MenuItem>("select * from menu_item where is_active = 1 and menu_category_type = " + (int)catType);
        }

        public List<Ingredient> GetIngredients(List<int> ids = null)
        {
            if(ids != null)
            {
                return genericRepository.Get<Ingredient>($"select * from ingredient where id in ({string.Join(",",ids)})");
            }
            else
            {
                return genericRepository.Get<Ingredient>("select * from ingredient");
            }
        }

        public MenuItem GetMenuItem(int id)
        {
            return genericRepository.Get<MenuItem>("select * from menu_item where id = " + id).First();
        }

        public List<MenuCategory> GetMenuCategorties(MenuCategoryType catType)
        {
            return genericRepository.Get<MenuCategory>("select * from menu_category where is_active = 1 and  menu_category_type = " + (int)catType);
        }

        private void AddCreditCardChargeIfNeeded(Order order, UIOrder uiOrder)
        {
            if (UserHelper.IsResident(order.user_id))
            {
                return;
            }

            if (order.pay_type_id == PayType.Credit && uiOrder.creditCardChargePercentage > 0)
            {
                decimal extraCharge = (decimal)order.total * (uiOrder.creditCardChargePercentage / 100);
                    
                //Add extra charge to credit bill and not cash if it's not in settings
                order.total += (double)extraCharge;
                order.credit_charge = extraCharge;
            }
        }

        private void AddOrderCashRegisterEvents(Order order, int orderId, UIOrder uiOrder, Staff.Entities.Staff staff)
        {
            if (order.pay_type_id == PayType.Credit && uiOrder.creditCardChargePercentage > 0)
            {

                if (CacheManager.Instance.AppSettings.AddCreditCardChargeToRegister)
                {
                    var lastEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                    CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                    {
                        event_date = DateTimeHelper.GetCurrentDateTime(),
                        event_type_id = EventType.CashRegisterAddFromOrder,
                        event_value = order.credit_charge,
                        staff_id = staff.id,
                        staff_name = staff.name,
                        current_register_amount = lastEvent.current_register_amount + order.credit_charge,
                        event_realted_entity_id = orderId
                    });
                }
            }

            if (order.pay_type_id == PayType.Cash)
            {
                var lastCashEvent = CashRegisterManager.Instance.GetLastEventOrDefault();
                CashRegisterManager.Instance.AddEvent(new CashRegisterEvent
                {
                    current_register_amount = lastCashEvent.current_register_amount + (decimal)order.total,
                    event_date = DateTimeHelper.GetCurrentDateTime(),
                    event_realted_entity_id = orderId,
                    event_type_id = EventType.CashRegisterAddFromOrder,
                    staff_id = staff.id,
                    staff_name = staff.name,
                    event_value = (decimal)order.total
                });
            }
        }
    
        private void UpdateProductsInvetory(OrderItems orderItem)
        {
            MenuItem menuItem = GetMenuItem(orderItem.menu_item_id);
            if (menuItem.product_id.HasValue && menuItem.product_weight > 0)
            {
                productService.AddOrRemoveQuantityAndUpdateStock(new ProductQuantityChange
                {
                    ProductId = menuItem.product_id.Value,
                    Value = -menuItem.product_weight,
                    Origin = ProductQuantityChangeOrigin.MENU_ORDER
                });
            }

            List<MenuItemIngredient> ingredients = GetMenuItemIngredient(orderItem.menu_item_id, orderItem.menu_item_ingredients_ids);
            if (ingredients.Any())
            {
                ingredients.ForEach((ingredient) =>
                {
                    if (ingredient.IsRelatedToProductAndHaveQuantity())
                    {
                        productService.AddOrRemoveQuantityAndUpdateStock(new ProductQuantityChange
                        {
                            ProductId = ingredient.product_id.Value,
                            Value = -ingredient.product_weight,
                            Origin = ProductQuantityChangeOrigin.MENU_ORDER
                        });
                    }
                });
            }
        }
    }
}