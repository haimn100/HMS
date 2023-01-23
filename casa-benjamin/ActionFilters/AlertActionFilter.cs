using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace casa_benjamin.ActionFilters
{
    public class AlertActionFilter : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            base.OnActionExecuting(context);

            var querycollection = context.RequestContext.HttpContext.Request.QueryString;

            if (!string.IsNullOrEmpty(querycollection["alert"]))
            {
                context.Controller.TempData["alert"] = querycollection["alert"];
                context.Controller.TempData["alerttype"] = querycollection["alerttype"];
                string query = string.Empty;
                foreach (string key in querycollection.AllKeys.Except(new List<string> { "alert", "alerttype" }))
                {
                    query += key + "=" + querycollection[key] + "&";
                }
                string url = context.RequestContext.HttpContext.Request.Url.AbsolutePath + (string.IsNullOrEmpty(query) ? "" : "?" + query);
                context.Result = new RedirectResult(url);
            }           
        }
    }
}