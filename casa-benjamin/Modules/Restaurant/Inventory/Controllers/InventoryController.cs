using casa_benjamin.ActionFilters;
using System.Collections.Generic;
using System.Web.Mvc;
using casa_benjamin.Modules.Restaurant.Inventory.Entities;
using casa_benjamin.Modules.Restaurant.Inventory.Services;
using System.Configuration;
using casa_benjamin.Modules.Restaurant.Inventory.Values;
using casa_benjamin.Modules.Restaurant.Inventory.Enums;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Values;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Kitchen.Services;

namespace casa_benjamin.Controllers
{
    [AuthenticateActionFilter(Roles = "Admin,Employee,Editor,KitchenManager")]
    public class InventoryController : Controller
    {
        private GenericRepository repository;
        private InventoryService inventoryService;

        public InventoryController()
        {
            inventoryService = new InventoryService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
            repository = new GenericRepository();
        }

        public ActionResult Consumption()
        {
            return View("~/Views/Admin/Product/Consumption.cshtml");
        }


        [HttpPost]
        public ActionResult UpdateInventory(Product product)
        {
            var updateRequest = new UpdateInventoryRequest
            {
                ActionType = ProductQuantityChangeOrigin.MANUAL,
                InventoryProductsToUpdate = new List<KeyValuePair<int, decimal>>
                {
                    { new KeyValuePair<int, decimal>(product.id, product.weight) }
                }
            };

            inventoryService.UpdateInventory(updateRequest);
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }


        [HttpGet]
        public ActionResult Suppliers()
        {
            var suppliers = repository.GetAll<Supplier>();
            return new JsonResult() { Data = suppliers, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult ConsumptionTable(PagedTableRequest req)
        {
            var data = KitchenManager.Instance.GetMenuItems(MenuCategoryType.Kitchen);
            var res = new PagedTableResponse<MenuItem>
            {
                data = data,
                recordsTotal = data.Count
            };
            return new JsonResult { Data = res, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        //[HttpPost]
        //public ActionResult ConsumptionUpdate(MenuItem m)
        //{
        //    repository.ExecuteScalar($"update menu_item set consumption = {m.consumption} where id ={m.id}");
        //    return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        //}
    }
}