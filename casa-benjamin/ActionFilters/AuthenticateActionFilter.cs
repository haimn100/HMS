using casa_benjamin.Modules.Staff.Entities;
using casa_benjamin.Modules.User.Entities;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.ActionFilters
{
    public class AuthenticateActionFilter : ActionFilterAttribute
    {
        public string Roles { get; set; }

        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

#if (DEBUG)
            context.HttpContext.Session["user"] = new Staff
            {
                name = "Admin - Test",
                type = UserType.Admin
            };
            return;
#endif

            var request = context.RequestContext.HttpContext.Request;
            if (request.Url.Scheme == "https")
            {
                if(
                    request.Url.AbsolutePath.ToLower() != "/guest/getreservationsindates"
                    && request.Url.AbsolutePath.ToLower() != "/guest/checkin" 
                    && request.Url.AbsolutePath.ToLower() != "/guest/checkinform" 
                    && request.Url.AbsolutePath.ToLower() != "/guest/getuserbypassport")
                {
                    context.Result = new RedirectResult(request.Url.AbsoluteUri.Replace("https","http"));
                }
            }
            
            //context.HttpContext.Session["user"] = new Staff { name = "debug", type = UserType.Admin };           

            if (context.HttpContext.Session["user"] == null)
            {
                string path = context.HttpContext.Request.Path.ToLower();
                if (path != "/account/login" && path != "/account/loginform")
                {
                    context.Result = new RedirectResult("~/Account/Login");
                }
            }
            else if(!string.IsNullOrEmpty(Roles))
            {
                string t = ((Staff)context.HttpContext.Session["user"]).type.ToString().ToLower();
                bool inRole = Roles.Split(',').Any(x => x.ToLower() == t);

                if (!inRole)
                {
                    context.Result = new HttpUnauthorizedResult();
                }
            }
        }
    }
}