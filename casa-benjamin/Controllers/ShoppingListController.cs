//using casa_benjamin.ActionFilters;
//using casa_benjamin.Data;
//using casa_benjamin.Helpers;
//using casa_benjamin.Managers;
//using casa_benjamin.Models;
//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Web;
//using System.Web.Mvc;

//namespace casa_benjamin.Controllers
//{
//    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor,KitchenManager")]
//    public class ShoppingListController : Controller
//    {
//        private GenericRepository repository = new GenericRepository();
       

//        public ActionResult Index(int? id)
//        {
//            if (!id.HasValue)
//            {
//                return View("~/Views/Admin/ShoppingList/All.cshtml");
//            }
//            ShoppingList model = repository.Get<ShoppingList>($"select * from shopping_list where id = {id}").First();
//            ViewBag.SupplierName = repository.Get<ExpenseCategory>($"select * from expense_category where id = {model.supplier_id}").First().name;
//            return View("~/Views/Admin/ShoppingList/ShoppingList.cshtml", model);
//        }

//        [HttpGet]
//        public ActionResult AllTable(PagedTableRequest req)
//        {
//            req.table = "shopping_list";
//            return new JsonResult() { Data = repository.GetTable<ShoppingList>(req), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
//        }

//        public ActionResult ProductsTable(int id)
//        {
//            var req = new PagedTableRequest() { 
//                table = "shopping_list_product",
//                predicate = $"shopping_list_id = {id}"
//            };
//            var data = repository.GetTable<ShoppingListProduct>(req);
//            return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
//        }

//        public ActionResult Recommend()
//        {
//            return View("~/Views/Admin/ShoppingList/Recommend.cshtml");
//        }

//        [HttpGet]
//        public ActionResult RecommendedTable(PagedTableRequest req, int supplier_id, int days)
//        {
//            var model = inventoryService.RecommendedProducts(supplier_id,days);
//            var res = new PagedTableResponse<ShoppingListProduct>
//            {
//                data = model,
//                recordsTotal = model.Count
//            };
//            return new JsonResult() { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
//        }

//        [HttpPost]
//        public ActionResult Create(int supplierId,int days = 1)
//        {
//            var products = InventoryService.Instance.RecommendedProducts(supplierId,days);
//            var data = ShoppingListManager.Instance.Create(products, supplierId,days);
//            return new JsonResult { Data = data };
//        }

//        [HttpPost]
//        public ActionResult Purchase(int id)
//        {
//            try
//            {
//                var data = ShoppingListManager.Instance.Purchase(id);
//                return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
//            }
//            catch (Exception ex)
//            {
//                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, ex.Message);
//            }
//        }

//        [HttpPost]
//        public void Delete(int id)
//        {
//            ShoppingListManager.Instance.Delete(id);
//        }

//        [HttpPost]
//        public void UpdateName(int id, string name)
//        {
//            repository.ExecuteScalar($"update shopping_list set name = '{name}', updated = NOW() where id = {id}");
//        }

//        [HttpPost]
//        public ActionResult AddProduct(ShoppingListProduct product)
//        {
//            repository.Insert(product);
//            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
//        }

//        [HttpPost]
//        public ActionResult DeleteProduct(ShoppingListProduct product)
//        {
//            repository.Delete(product);
//            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
//        }

//        [HttpPost]
//        public ActionResult UpdateProduct(ShoppingListProduct product)
//        {
//            repository.Update(product);
//            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
//        }

      
//    }
//}