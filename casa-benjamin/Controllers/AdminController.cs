using casa_benjamin.ActionFilters;
using casa_benjamin.Internalization;
using casa_benjamin.Models;
using casa_benjamin.Modules.App.Values;
using casa_benjamin.Modules.Booking.Room.Entities;
using casa_benjamin.Modules.Booking.Room.Enums;
using casa_benjamin.Modules.Kitchen.Services;
using casa_benjamin.Modules.Restaurant.Menu.Entities;
using casa_benjamin.Modules.Shared.Services;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    public class AdminController : Controller
    {
        // GET: Admin
        public ActionResult Index()
        {          
            return Redirect("/reports/dailydashboard");
        }

        public ActionResult AppSettings()
        {
            return View();
        }

        public ActionResult shit()
        {
            return Json(I18n.shit,JsonRequestBehavior.AllowGet);
        }

        [AuthenticateActionFilter(Roles = "Admin")]
        [HttpPost, ValidateInput(false)]
        public ActionResult SaveSettings(AppSettings appSettings)
        {
            AppSettings settings = new AppSettings
            {
                id = CacheManager.Instance.AppSettings.id,
                AddCreditCardChargeToRegister = appSettings.AddCreditCardChargeToRegister,
                barcodePrefix = appSettings.barcodePrefix,
                CheckInDefaultNationality = appSettings.CheckInDefaultNationality,
                RequireImmigrationInfo = appSettings.RequireImmigrationInfo,
                CountryIsoAlpha2Code = CacheManager.Instance.AppSettings.CountryIsoAlpha2Code,
                language = appSettings.language,
                city = appSettings.city,
                hotel_code = appSettings.hotel_code,
                hotel_print_info = appSettings.hotel_print_info
            };

            UserManager.Instance.UpdateSettings(settings);
            CacheManager.Instance.RefreshAppSettings();
            return RedirectToAction("AppSettings");

        }

        #region Employees
        public ActionResult Employees()
        {
            return View("~/Views/Admin/Employees/Index.cshtml");
        }

        public ActionResult EmployeesAdd()
        {
            return View("~/Views/Admin/Employees/Add.cshtml");
        }

        public ActionResult EmployeeDelete(int id)
        {
            UserManager.Instance.DeleteStaff(id);
            CacheManager.Instance.RefreshStaff();
            return Employees();
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult EmployeesAdd(Staff employee)
        {
            employee.is_working = true;
            employee.start_date = DateTime.Now;
            ViewBag.Message = "Successfully Added Employee";
            UserManager.Instance.AddStaff(employee);
            CacheManager.Instance.RefreshStaff();
            return View("~/Views/Admin/Employees/Add.cshtml");
        }

        public ActionResult EmployeeEdit(int id, bool isUpdated = false)
        {
            if (isUpdated)
            {
                ViewBag.Message = "Successfully Updated Employee";
            }
            return View("~/Views/Admin/Employees/Edit.cshtml", UserManager.Instance.GetStaff(id));
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult EmployeesEditForm(Staff employee)
        {
            Staff dbEmployee = UserManager.Instance.GetStaff(employee.id);
            employee.start_date = dbEmployee.start_date;
            employee.password = dbEmployee.password;
            employee.pin = dbEmployee.pin;
            UserManager.Instance.UpdateStaff(employee);
            CacheManager.Instance.RefreshStaff();
            return RedirectToAction("Employees");
        }
        #endregion

        #region Menu
        public ActionResult MenuReport(ReportDateQuery dateQuery)
        {
            ViewBag.DateQuery = dateQuery;
            return View("~/Views/Admin/Reports/MenuReport.cshtml");
        }


        public ActionResult Categories()
        {
            return View("~/Views/Admin/Categories/Index.cshtml");
        }

        public ActionResult Ingredients(string error = "")
        {
            ViewBag.Error = error;
            return View("~/Views/Admin/Categories/Ingredients.cshtml");
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult IngredientEdit(Ingredient item)
        {
            KitchenManager.Instance.UpdateIngredient(item);
            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("Ingredients");
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult IngredientAdd(Ingredient item)
        {
            KitchenManager.Instance.InsertIngredient(item);
            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("Ingredients");
        }


        public ActionResult IngredientDelete(int id)
        {
            var items = KitchenManager.Instance.GetMenuItemIngredientsByIngredientId(id);

            if (items.Count > 0)
            {
                return RedirectToAction("Ingredients", new { error = "Cannot delete ingredient. Ingredient is being used by some Menu Items" });
            }
            else
            {
                KitchenManager.Instance.DeleteIngredient(new Ingredient { id = id });
                CacheManager.Instance.RefreshCategories();
                return RedirectToAction("Ingredients");
            }

        }

        public ActionResult CategoriesMenuItems(int cat)
        {
            var model = CacheManager.Instance.MenuCategories.First(x => x.category.id == cat);
            return View("~/Views/Admin/Categories/MenuItems.cshtml",model);
        }

        public ActionResult CategoriesMenuItemIngredients(int catid, int menuItemId)
        {
            MenuItem mitem = KitchenManager.Instance.GetMenuItem(menuItemId);
            List<MenuItemIngredient> ings = KitchenManager.Instance.GetMenuItemIngredientsByMenuItemId(menuItemId);

            ViewBag.CatId = catid;
            ViewBag.MenuItem = mitem;

            return View("~/Views/Admin/Categories/MenuItemIngredients.cshtml", ings);
        }

        public void UpdateMenuItemIngredientsGroup(string groupName, bool isSingleSelect,int menuItemId, int groupNumber)
        {
            KitchenManager.Instance.UpdateMenuItemIngredientGroup(groupName, isSingleSelect, menuItemId, groupNumber);
            CacheManager.Instance.RefreshCategories();
        }

        public void CategoriesMenuItemIngredientEdit(int id, double price)
        {
            var ingItem = KitchenManager.Instance.GetMenuItemIngredient(id);
            ingItem.ingredient_price = price;
            KitchenManager.Instance.UpdateMenuItemIngredient(ingItem);
            CacheManager.Instance.RefreshCategories();
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult CategoriesMenuItemIngredientAdd(int catid, MenuItemIngredient item)
        {
            long id = KitchenManager.Instance.InsertMenuItemIngredient(item);
            CacheManager.Instance.RefreshCategories();
            return Json(id, JsonRequestBehavior.AllowGet);
        }

        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public void CategoriesMenuItemIngredientDelete(int id)
        {
            KitchenManager.Instance.DeleteMenuItemIngredient(new MenuItemIngredient { id = id });
            CacheManager.Instance.RefreshCategories();
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult CategoriesMenuItemEdit(MenuItem mi, bool? putInStock)
        {
            KitchenManager.Instance.UpdateMenuItem(mi);

            //StockItem stockItem = StockManager.Instance.GetStockItemByMenuItemId(mi.id);
            //if (putInStock.HasValue && putInStock.Value && stockItem == null)
            //{
            //    StockManager.Instance.AddStockItem(new StockItem
            //    {
            //        menu_item_id = mi.id,
            //        menu_item_name = mi.name,
            //        menu_item_category_name = CacheManager.Instance.MenuCategories.First(x => x.category.id == mi.cat_id).category.name,
            //        menu_item_number = mi.number,
            //        quantity = 0
            //    });
            //}
            //else if (!putInStock.HasValue && stockItem != null)
            //{
            //    StockManager.Instance.RemoveStockItem(stockItem);
            //}

            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("CategoriesMenuItems",new { cat = mi.cat_id });
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]

        public ActionResult CategoriesMenuItemAdd(MenuItem mi,bool? putInStock)
        {
            KitchenManager.Instance.InsertMenuItem(mi);

            //if(mi.menu_category_type == MenuCategoryType.Kitchen)
            //{
            //    StockManager.Instance.AddStockItem(new StockItem
            //    {
            //        menu_item_id = mi.id,
            //        menu_item_name = mi.name,
            //        menu_item_number = mi.number,
            //        menu_item_category_name = CacheManager.Instance.MenuCategories.First(x => x.category.id == mi.cat_id).category.name,
            //        quantity = 0
            //    });
            //}

            //StockItem stockItem = StockManager.Instance.GetStockItemByMenuItemId(mi.id);
            //if (putInStock.HasValue && putInStock.Value && stockItem == null)
            //{
            //    StockManager.Instance.AddStockItem(new StockItem
            //    {
            //        menu_item_id = mi.id,
            //        menu_item_name = mi.name,
            //        menu_item_category_name = CacheManager.Instance.MenuCategories.First(x => x.category.id == mi.cat_id).category.name,
            //        menu_item_number = mi.number,
            //        quantity = 0
            //    });
            //}
            //else if (!putInStock.HasValue && stockItem != null)
            //{
            //    StockManager.Instance.RemoveStockItem(stockItem);
            //}

            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("CategoriesMenuItems", new { cat = mi.cat_id });
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]

        public ActionResult CategoriesAdd(MenuCategory cat)
        {
            KitchenManager.Instance.InsertMenuCategory(cat);
            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("Categories");
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor,KitchenManager")]
        public ActionResult CategoriesEdit(MenuCategory cat)
        {
            MenuCategory _cat = KitchenManager.Instance.GetMenuCategory(cat.id);
            _cat.name = cat.name;
            _cat.number = cat.number;

            KitchenManager.Instance.UpdateCategory(_cat);
            CacheManager.Instance.RefreshCategories();
            return RedirectToAction("Categories");
        }

        //public ActionResult CategoriesDisable(int id)
        //{
        //    UserManager.Instance.DeleteStaff(id);
        //    return Employees();
        //}

        //[HttpPost]
        //public ActionResult CategoriesAdd(UIMenuCategory cat)
        //{
        //    UserManager.Instance.AddStaff(employee);
        //    return View("~/Views/Admin/Employees/Add.cshtml");
        //}



        //[HttpPost]
        //public ActionResult EmployeesEditForm(Staff employee)
        //{
        //    UserManager.Instance.UpdateStaff(employee);
        //    return RedirectToAction("EmployeeEdit", new { id = employee.id, isUpdated = true });
        //}
        #endregion

        #region Rooms
        public ActionResult Rooms()
        {
            return View("~/Views/Admin/Rooms/Index.cshtml");
        }

        public ActionResult Beds()
        {
            return View("~/Views/Admin/Rooms/Beds.cshtml");
        }

        public ActionResult RoomBeds(int roomId)
        {
            var model = UserManager.Instance.GetRoomBeds(roomId);
            ViewBag.RoomId = roomId;
            return View("~/Views/Admin/Rooms/RoomBeds.cshtml",model);
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult RoomBedAdd(int roomId,int bedId)
        {
            Bed bed = UserManager.Instance.GetBed(bedId);
            RoomBed exisingRoomBed = UserManager.Instance.GetRoomBed(bedId);
            if(exisingRoomBed != null)
            {               
                UserManager.Instance.DeleteRoomBed(exisingRoomBed);
                if (exisingRoomBed.double_bed_partner_id.HasValue)
                {
                    var b = UserManager.Instance.GetRoomBed(exisingRoomBed.double_bed_partner_id.Value);
                    if(b != null)
                    {
                        UserManager.Instance.DeleteRoomBed(b);
                    }
                }
            }

            if(bed.bed_type_id == BedType.Double || bed.bed_type_id == BedType.BubkBed)
            {
                Bed partner = UserManager.Instance.GetBed((int)bed.double_bed_partner_id);
                UserManager.Instance.InsertRoomBed(new RoomBed
                {
                    room_id = roomId,
                    bed_id = partner.id,
                    bed_type_id = partner.bed_type_id,
                    double_bed_partner_id = partner.double_bed_partner_id
                });
                UserManager.Instance.InsertRoomBed(new RoomBed
                {
                    bed_id = bedId,
                    room_id = roomId,
                    bed_type_id = bed.bed_type_id,
                    double_bed_partner_id = bed.double_bed_partner_id
                });
            }
            else
            {
                UserManager.Instance.InsertRoomBed(new RoomBed
                {
                    bed_id = bedId,
                    room_id = roomId,
                    bed_type_id = bed.bed_type_id,
                    double_bed_partner_id = bed.double_bed_partner_id
                });
            }
            CacheManager.Instance.RefreshRooms();
            return RedirectToAction("RoomBeds", new { roomId });
        }

        public ActionResult RoomBedDelete(int currentRoomId,int roomBedId,int bedId)
        {
            Bed bed = UserManager.Instance.GetBed(bedId);

            if (bed.bed_type_id == BedType.Double)
            {
                Bed partner = UserManager.Instance.GetBed((int)bed.double_bed_partner_id);
                RoomBed roomBedPartner = UserManager.Instance.GetRoomBed(partner.id);

                UserManager.Instance.DeleteRoomBed(new RoomBed { id = roomBedId });
                if(roomBedPartner != null)
                {
                    UserManager.Instance.DeleteRoomBed(roomBedPartner);
                }
            }
            else
            {
                UserManager.Instance.DeleteRoomBed(new RoomBed { id = roomBedId });
            }

            CacheManager.Instance.RefreshRooms();
            return RedirectToAction("RoomBeds", new { roomId  = currentRoomId });
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult RoomEdit(Room room)
        {
            room.amenities = Request.Form["amenities"];
            UserManager.Instance.UpdateRoom(room);
            CacheManager.Instance.RefreshRooms();
            return RedirectToAction("Rooms");
        }

        [HttpPost]
        [AuthenticateActionFilter(Roles = "Admin,Editor")]
        public ActionResult BedEdit(Bed bed)
        {
            Bed selectedBed = UserManager.Instance.GetBed(bed.id);

            if ((bed.bed_type_id == BedType.Double || bed.bed_type_id == BedType.BubkBed))
            {
                if(selectedBed.bed_type_id == BedType.Single)
                {
                    Bed bedPartner = UserManager.Instance.GetBed(bed.id + 1);
                    if ((bedPartner.bed_type_id == BedType.Double || bedPartner.bed_type_id == BedType.BubkBed))
                    {
                        throw new Exception("The next bed is also a double or bunk bed");
                    }

                    bed.double_bed_partner_id = bedPartner.id;
                    bedPartner.double_bed_partner_id = bed.id;
                    bedPartner.bed_type_id = bed.bed_type_id;
                    UserManager.Instance.UpdateBed(bed);
                    UserManager.Instance.UpdateBed(bedPartner);
                }
                else
                {
                    Bed partner = UserManager.Instance.GetBed((int)selectedBed.double_bed_partner_id);
                    partner.bed_type_id = bed.bed_type_id;
                    bed.double_bed_partner_id = partner.id;
                    UserManager.Instance.UpdateBed(partner);
                    UserManager.Instance.UpdateBed(bed);

                }
            }
            else
            {
                if(selectedBed.bed_type_id == BedType.Double || selectedBed.bed_type_id == BedType.BubkBed)
                {
                    Bed partner = UserManager.Instance.GetBed((int)selectedBed.double_bed_partner_id);
                    partner.double_bed_partner_id = null;
                    partner.bed_type_id = BedType.Single;
                    bed.double_bed_partner_id = null;
                    UserManager.Instance.UpdateBed(bed);
                    UserManager.Instance.UpdateBed(partner);
                }
                else
                {
                    UserManager.Instance.UpdateBed(bed);
                }
            }

            CacheManager.Instance.RefreshBeds();
            CacheManager.Instance.RefreshRooms();
            return RedirectToAction("Beds");
        }

        #endregion  
    }
}