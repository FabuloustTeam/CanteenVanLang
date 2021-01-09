using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using CanteenVanLang.Models;

namespace CanteenVanLang.Areas.Customer.Controllers
{
    public class CustomerController : Controller
    {
        private QUANLYCANTEENEntities db = new QUANLYCANTEENEntities();
        // GET: Customer/Auth
        public ActionResult SignUp()
        {
            return View(db.FACULTies.ToList());
        }

        [HttpPost]
        public ActionResult SignUp(string name, string password, string confirmPassword, string email, string numberPhone, int faculty)
        {
            if (password != confirmPassword)
            {
                ModelState.AddModelError("confirmFail", "Mật khẩu xác nhận không khớp");
                return SignUp();
            }
            if (name == "" || password == "" || confirmPassword == "" || email == "" || numberPhone == "")
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
                return SignUp();
            }
            if (!checkEmail(email))
            {
                ModelState.AddModelError("invalidEmail", "Email không hợp lệ");
                return SignUp();
            }
            if (db.CUSTOMERs.Count(item => item.EMAIL == email) == 1)
            {
                ModelState.AddModelError("existedEmail", "Email đã tồn tại");
                return SignUp();
            }
            CUSTOMER cust = new CUSTOMER();
            cust.EMAIL = email;
            cust.FULLNAME = name;
            cust.FACULTY_ID = faculty;
            cust.PASSWORD = password;
            cust.PHONE = numberPhone;
            cust.STATUS = false;
            cust.MEMBER_TYPE = 0;
            db.CUSTOMERs.Add(cust);
            db.SaveChanges();
            ViewBag.SignInSucceed = true;
            return SignUp();
        }

        private bool checkEmail(string email)
        {
            Regex regex = new Regex("^[a-zA-Z0-9_.+-]+@[a-zA-Z0-9-]+.[a-zA-Z0-9-.]+$");
            return regex.IsMatch(email);
        }
         
        public ActionResult LogIn()
        {
            return View();
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {
            CUSTOMER cust = db.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email && item.PASSWORD == password);
            if (cust == null)
            {
                ModelState.AddModelError("loginFail", "Email hoặc mật khẩu không đúng");
                return LogIn();
            }
            else
            {
                Session["customerEmail"] = cust.EMAIL;
                Session["customerName"] = cust.FULLNAME;
            }
            return Redirect("/");
        }
    }
}