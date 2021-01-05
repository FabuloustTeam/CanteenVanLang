using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanteenVanLang.Areas.Admin.Middleware
{
    public class LoginVertification : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if(filterContext.HttpContext.Session["userId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Admin/Authentication/Login");
                return;
            }
        }

    }
}