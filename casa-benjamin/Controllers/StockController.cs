//using casa_benjamin.ActionFilters;
//using casa_benjamin.Models;
//using casa_benjamin.Modules.Shared.Services;
//using System;
//using System.Web.Mvc;

//namespace casa_benjamin.Controllers
//{
//    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor,KitchenManager")]
//    public class StockController : Controller
//    {
//        // GET: Stock
//        public ActionResult Index()
//        {
//            var model = StockManager.Instance.GetAllStockItems();

//            return View("~/Views/Admin/Stock/Index.cshtml",model);
//        }

//        public void UpdateStockItemQuantity(int id, int quantity)
//        {
//            StockItem sItem = StockManager.Instance.GetStockItem(id);
//            sItem.quantity += quantity;
//            StockManager.Instance.UpdateStockItem(sItem);
//            CacheManager.Instance.RefreshStockAlerts();

//            new GenericRepository().Insert(new StockHistoryItem
//            {
//                menu_item_category_name = sItem.menu_item_category_name,
//                menu_item_id = sItem.menu_item_id,
//                menu_item_name = sItem.menu_item_name,
//                menu_item_number = sItem.menu_item_number,
//                quantity = quantity,
//                total = sItem.quantity,
//                timestamp = DateTime.Now
//            });

//        }

//        public void UpdateStockItemWarningQuantity(int id, int quantity)
//        {
//            StockItem sItem = StockManager.Instance.GetStockItem(id);
//            sItem.warning_quantity = quantity;
//            StockManager.Instance.UpdateStockItem(sItem);
//            CacheManager.Instance.RefreshStockAlerts();

//        }

//        public void DeleteStock(int id)
//        {
//            var item =  StockManager.Instance.GetStockItem(id);
//            StockManager.Instance.DeleteStockItem(item);
//        }



//    }
//}