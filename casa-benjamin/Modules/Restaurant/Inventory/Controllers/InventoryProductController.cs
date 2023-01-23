using casa_benjamin.ActionFilters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using casa_benjamin.Modules.Restaurant.Inventory.Entities;
using casa_benjamin.Modules.Restaurant.Inventory.Services;
using System.Configuration;
using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using casa_benjamin.Modules.Inventory.Enums;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using casa_benjamin.Modules.Restaurant.Inventory.Values;

namespace casa_benjamin.Controllers
{
    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor,KitchenManager")]
    public class InventoryProductController : Controller
    {
        private GenericRepository repository;
        private InventoryService inventoryService;
        private ProductService productsService;

        private const string PRODUCTS_TABLE = "restaurant_inventory_product";

        public InventoryProductController()
        {
            inventoryService = new InventoryService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
            productsService = new ProductService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
            repository = new GenericRepository();
        }

        public ActionResult Index()
        {
            return View("~/Views/Inventory/Products.cshtml");
        }

        public ActionResult StockPage()
        {
            return View("~/Views/Inventory/Stock.cshtml");
        }

        [HttpGet]
        public ActionResult AllItemsTable(PagedTableRequest req,string show_only, int? supplier_id)
        {         
            var filters = new List<string>();
            
            if(show_only == "warnings")
            { 
                filters.Add("quantity_in_stock < quantity_warning_thershold");
            }

            if (supplier_id.HasValue)
            {
                filters.Add($"supplier_id = {supplier_id.Value}");
            }

            if (filters.Any())
            {
                req.predicate = string.Join(" and ", filters);
            }

            if (!string.IsNullOrWhiteSpace(req.search))
            {
                req.sortBy = "name";
            }

            var data = productsService.Table(req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult SelectList(int? supplierId = null)
        {

            List<Product> data = supplierId.HasValue ? 
                productsService.FindBySupllier(supplierId.Value): productsService.FindAll();
            
            data.ForEach(x => { x.name = string.IsNullOrEmpty(x.brand) ? x.name : x.name + " (" + x.brand + ")"; });

            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        public ActionResult AddProduct(Product product)
        {
            int pid = productsService.AddProduct(product);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK, pid.ToString());
        }

        [HttpPost]
        public ActionResult DeleteProduct(Product product)
        {
            productsService.DeleteAndArchiveProduct(product.id);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult UpdateProductWarningQuantity(int id, decimal quantity_warning_thershold)
        {
            productsService.UpdateProductWarningQuantity(id, quantity_warning_thershold);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult UpdateProduct(Product product)
        {
            var dbProduct = productsService.FindOne(product.id);

            dbProduct.note = product.note;
            dbProduct.price = product.price;
            dbProduct.weight = product.weight;
            dbProduct.brand = product.brand;
            dbProduct.code = product.code;
            dbProduct.supplier_id = product.supplier_id;
            
            if(dbProduct.name != product.name)
            {
                if(productsService.IsDuplicateProduct(product.name, product.code))
                {
                    throw new Exception("Product name already exists");
                }
            }

            repository.Update(dbProduct);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public int AddQuantityToProduct(AddQuantityToProductRequest addRequest)
        {
            return productsService.AddOrRemoveQuantityAndUpdateStock(new ProductQuantityChange
            {
                Origin = ProductQuantityChangeOrigin.MANUAL,
                ProductId = addRequest.productId,
                Value = addRequest.quantity,
                ProductUnit = ProductUnit.GRAMS,
                Note = addRequest.note
            });
        }


        //[HttpGet]
        //public ActionResult AssignTable(PagedTableRequest req)
        //{
        //    req.table = "inventory_product_assign";
        //    var data = repository.GetTable<ProductAssign>(req);
        //    return new JsonResult { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}


        //[HttpPost]
        //public ActionResult AssignAdd(ProductAssign p)
        //{
        //    repository.Insert(p);
        //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        //}

        //[HttpPost]
        //public ActionResult AssignDelete(ProductAssign p)
        //{
        //    repository.Delete(p);
        //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        //}

        //[HttpPost]
        //public ActionResult AssignUpdate(ProductAssign p)
        //{
        //    repository.Update(p);
        //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        //}      

        //[HttpGet]
        //public ActionResult MenuItemProductsTable(PagedTableRequest req)
        //{
        //    req.table = "menu_item_product";
        //    var data = repository.GetTable<ProductAssign>(req);
        //    return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        //}
    }
}