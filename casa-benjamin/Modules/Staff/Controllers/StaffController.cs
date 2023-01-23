using casa_benjamin.Modules.Staff.Services;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace casa_benjamin.Modules.Staff.Controllers
{
    public class StaffController : Controller
    {
        private StaffService staffService = new StaffService(ConfigurationManager.ConnectionStrings["casa-benjamin"].ConnectionString);
        

        public ActionResult All()
        {
            return new JsonResult { Data = staffService.All(), JsonRequestBehavior = JsonRequestBehavior.AllowGet };
        }
    }
}