using casa_benjamin.ActionFilters;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Restaurant.Menu.Services;
using casa_benjamin.Modules.Restaurant.Menu.Values;
using casa_benjamin.Modules.Shared.Repositories;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Shared.Values;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Modules.Restaurant.Menu.Contollers
{
    public class MenuController : Controller
    {

        private MenuService menuService;
        private GenericRepository repository;

        public MenuController()
        {
            repository = new GenericRepository();
            menuService = new MenuService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
        }

        public ActionResult Items(int cat)
        {
            MenuCategory category = menuService.GetCategory(cat);
            return View("~/Views/Menu/Items.cshtml",category);
        }

        public ActionResult Categories()
        {
            return View("~/Views/Menu/Categories.cshtml");
        }

        public ActionResult MenuItemIngredientsPage(int menuItemId)
        {
            MenuItem menuItem = menuService.GetMenuItem(menuItemId);
            ViewBag.CategoryName = menuService.GetCategory(menuItem.cat_id).name;
            return View("~/Views/Menu/MenuItemIngredients.cshtml", menuItem);
        }

        // GET: Menu
        public ActionResult GetMenuItemIngredients(int menuItemId)
        {
            return new JsonResult
            {
                Data = menuService.GetMenuItemIngredients(menuItemId),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult GetMenuItemIngredientsGroups(int id)
        {
            var data = menuService.GetMenuItemIngredients(id);
            if (data.Any())
            {
                var groups = data.GroupBy(x => x.ingredients_group_number);
                var result = groups.Select(x =>
                {
                    var first = x.First();
                    return new
                    {
                        position = first.ingredients_group_number,
                        name = first.ingredients_group,
                        is_single_select = first.ingredients_group_single_select,
                        items = x.ToList()
                    };
                });

                return new JsonResult
                {
                    Data = result,
                    JsonRequestBehavior = JsonRequestBehavior.AllowGet
                };
            }

            return new JsonResult
            {
                Data = data,
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        public ActionResult GetMenuItemIngredientsTable(PagedTableRequest req, int? menuItemId, int? groupPosition)
        {
            req.table = "menu_item_ingredient";
            List<string> predicates = new List<string>();
            
            if (menuItemId.HasValue){ predicates.Add($"menu_item_id = {menuItemId.Value}"); }
            if (groupPosition.HasValue) { predicates.Add($"ingredients_group_number = {groupPosition.Value}"); }
            if (predicates.Any())
            {
                req.predicate = string.Join(" and ",predicates);
            }

            var data = repository.GetTable<MenuItemIngredient>(req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        public ActionResult MenuItemsList()
        {        
            return new JsonResult
            {
                Data = menuService.GetActiveMenuItems(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpGet]
        public ActionResult ItemsTable(PagedTableRequest req, int? categoryId)
        {
            req.table = "menu_item";
            if (categoryId.HasValue) req.predicate = $"cat_id = {categoryId.Value} and is_deleted = 0";

            var data = repository.GetTable<MenuItem>(req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpGet]
        public ActionResult CategoriesTable(PagedTableRequest req)
        {
            req.table = "menu_category";
            req.predicate = "is_deleted = 0";
            var data = repository.GetTable<MenuItem>(req);
            return new JsonResult() { Data = data, JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult AddItem(MenuItem item)
        {
            try
            {
                int pid = menuService.AddMenuItem(item);
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK, pid.ToString());
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult AddMenuCategory(MenuCategory item)
        {
            try
            {
                int pid = menuService.AddMenuItemCategory(item);
                CacheManager.Instance.RefreshCategories();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK, pid.ToString());
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }


        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult UpdateItem(UpdateMenuItemDTO item)
        {
            try
            {
                menuService.UpdateMenuItem(item);
                CacheManager.Instance.RefreshCategories();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult UpdateCategory(MenuCategory category)
        {
            try
            {
                menuService.UpdateMenuCategory(category);
                CacheManager.Instance.RefreshCategories();
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
            }
            catch (Exception ex)
            {
                return new HttpStatusCodeResult(System.Net.HttpStatusCode.InternalServerError, ex.Message);
            }
        }

        public ActionResult IngredientsList()
        {
            return new JsonResult
            {
                Data = menuService.GetIngredients(),
                JsonRequestBehavior = JsonRequestBehavior.AllowGet
            };
        }

        [HttpPost]
        public ActionResult DeleteMenuItemIngredient(int id)
        {
            var item = menuService.FindMenuItemIngredientById(id);
            repository.Delete(item);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult DeleteMenuItem(int id)
        {
            var item = menuService.GetMenuItem(id);
            item.is_active = false;
            item.is_deleted = true;
            repository.Update(item);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult DeleteCategory(int id)
        {
            var item = menuService.GetCategory(id);
            item.is_active = false;
            item.is_deleted = true;
            repository.Update(item);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult UpdateMenuItemIngredient(MenuItemIngredient item)
        {
            var dbItem = menuService.FindMenuItemIngredientById(item.id);
            
            dbItem.ingredient_price = item.ingredient_price;
            dbItem.product_id = item.product_id.HasValue ? item.product_id.Value : item.product_id;
            dbItem.product_weight = item.product_weight;

            repository.Update(dbItem);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult UpdateMenuItemIngredientGroup(MenuItemIngredient item)
        {
            menuService.UpdateIngredientsMenuGroup(item);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }

        [HttpPost]
        public ActionResult AddMenuItemIngredient(MenuItemIngredient item)
        {
            repository.Insert(item);
            CacheManager.Instance.RefreshCategories();
            return new HttpStatusCodeResult(System.Net.HttpStatusCode.OK);
        }
    }
}