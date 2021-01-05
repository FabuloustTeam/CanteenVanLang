using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Areas.Admin.Middleware;
using CanteenVanLang.Models;

namespace CanteenVanLang.Areas.Admin.Controllers
{
    public class AuthenticationController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();

        // GET: Admin/Authentication
        public ActionResult Login()
        {
            Session["incorrectPassword"] = false;
            Session["userNotFound"] = false;
            Session["deactive"] = false;
            return View();
        }

        [HttpPost]
        public ActionResult Login(string email, string password)
        {
            Session["incorrectPassword"] = false;
            Session["userNotFound"] = false;
            Session["deactive"] = false;
            ValidateLogin(email, password);
            if (ModelState.IsValid)
            {
                var account = model.ACCOUNTs.FirstOrDefault(acc => acc.EMAIL.Equals(email));
                if (account != null)
                {
                    if (account.PASSWORD.Equals(password))
                    {
                        if (!account.STATUS)
                        {
                            Session["deactive"] = true;
                            return View();
                        }
                        else
                        {
                            Session["userFullName"] = account.FULLNAME;
                            Session["userId"] = account.ID;
                            Session["userRole"] = account.ROLE;
                            Session["userImage"] = account.IMAGE_URL;
                            if ((int)Session["userRole"] == 3)
                            {
                                return RedirectToAction("Welcome");
                            }
                            return RedirectToAction("Welcome", "Authentication");
                        }
                    }
                    else
                    {
                        Session["incorrectPassword"] = true;
                        ViewBag.email = account.EMAIL;
                        return View();
                    }
                }
                Session["userNotFound"] = true;
                return View();
            }
            return View();
        }

        private void ValidateLogin(string email, string password)
        {
            if(email.Trim() == "")
            {
                ModelState.AddModelError("EMAIL", "Vui lòng nhập email");
            }
            if (password.Trim() == "")
            {
                ModelState.AddModelError("PASSWORD", "Vui lòng nhập mật khẩu");
            }
        }

        public ActionResult Logout()
        {
            Session["userFullName"] = null;
            Session["userId"] = null;
            Session["userRole"] = null;
            return RedirectToAction("Login", "Authentication");
        }
        
        [LoginVertification]
        public ActionResult Welcome()
        {
            return View();
        }
        
    }
}