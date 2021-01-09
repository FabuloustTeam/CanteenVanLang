using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanteenVanLang.Areas.Admin.Middleware
{
    public class PermissionVertification : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if ((int) filterContext.HttpContext.Session["userRole"] == 3)
            {
                filterContext.Result = new RedirectResult("~/Admin/Authentication/Welcome");
                return;
            }
        }
    }
}