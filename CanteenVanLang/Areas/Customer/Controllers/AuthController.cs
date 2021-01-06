using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CanteenVanLang.Areas.Customer.Controllers
{
    public class AuthController : Controller
    {
        // GET: Customer/Auth
        public ActionResult SignUp()
        {

            return View();
        }

        [HttpPost]
        public ActionResult SignUp(String name, String password, String confirmPassword, String email, String numberPhone, String faculty)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("confirmFail", "Mật khẩu xác nhận không khớp");
                return View();
            }
            if (name == "" || password == "" || confirmPassword == "" || email == "" || numberPhone == "" || faculty == "")
            {
               if (name == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Họ và tên");
               else if (password == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Mật khẩu");
               else if (confirmPassword == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Xác nhận mật khẩu");
               else if (email == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Email");
               else if (numberPhone == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Số điện thoại");
               else if (faculty == "")
                ModelState.AddModelError("informationMissing", "Vui lòng nhập Khoa");
                return View();


            }
            return RedirectToAction("Index", "home");
        }
    }
}