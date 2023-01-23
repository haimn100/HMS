using casa_benjamin.ActionFilters;
using casa_benjamin.Helpers;
using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using casa_benjamin.Modules.User.Services;
using System.Configuration;
using System.Web.Mvc;

namespace casa_benjamin.Controllers
{
    public class AccountController : Controller
    {
        // GET: Account
        public ActionResult Login()
        {
            ViewBag.Error = null;
            return View();
        }

        public ActionResult LogOut()
        {
            Session.Remove("user");
            return Redirect("/");
        }

        [HttpPost]
        public ActionResult LoginForm()
        {
            string email = HttpContext.Request.Form["user"];
            string password = HttpContext.Request.Form["password"];

            if(email == ConfigurationManager.AppSettings["admin"] && password == ConfigurationManager.AppSettings["adminPassword"])
            {
                Session["user"] = new Staff
                {
                    name = "Admin",
                    email = email,
                    is_working = true,
                    type = UserType.Admin
                };
                return Redirect("/");
            }

            Staff user = UserManager.Instance.GetStaff(email);

            if (user != null && CryptographyHelper.VerifyPassword(password, user.password))
            {
                Session["user"] = user;

                if(user.type == UserType.HouseKeeper)
                {
                    return Redirect("/housekeeping");
                }
                else
                {
                    return Redirect("/");
                }
            }
            else
            {
                ViewBag.Error = true;
                return View("~/Views/Account/Login.cshtml");
            }
        }

        [AuthenticateActionFilter(Roles = "Admin")]
        public void AddStaff(Staff memeber)
        {
            memeber.password = CryptographyHelper.HashPassword(memeber.password);
            UserManager.Instance.AddStaff(memeber);
        }
    }
}