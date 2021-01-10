using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanteenVanLang.Controllers
{
    public class CustomerVerification : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            if (filterContext.HttpContext.Session["customerId"] == null)
            {
                filterContext.Result = new RedirectResult("~/Customer/Login");
                return;
            }
        }
    }
}