using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.User.Services;
using System;
using System.Collections.Generic;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    public class ImmigrationController : Controller
    {
        // GET: Immigration
        public ActionResult Index(DateTime? date)
        {
            DateTime _date = date.HasValue ? date.Value : DateTime.Now;
            ViewBag.Date = _date;
            List<User> model = UserManager.Instance.GetImmigrationUsers(_date); 
            return View("~/Views/Admin/Immigration/Index.cshtml",model);
        }
    }
}