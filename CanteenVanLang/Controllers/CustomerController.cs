using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Text.RegularExpressions;
using CanteenVanLang.Models;
using System.Dynamic;

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
        public ActionResult SignUp(string name, string password, string confirmPassword, string email, string numberPhone, int? faculty)
        {
            var temp = db.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email);
            CUSTOMER customer = new CUSTOMER() { FACULTY_ID = faculty, FULLNAME = name, PASSWORD = password, EMAIL = email, PHONE = numberPhone };
            if (!ValidateCustomerInfo(customer, confirmPassword))
            {
                return SignUp();
            }
            if (db.CUSTOMERs.Count(item => item.EMAIL == email) == 1)
            {
                ModelState.AddModelError("existedEmail", "Email đã tồn tại");
                return SignUp();
            }
            CUSTOMER cust = new CUSTOMER
            {
                EMAIL = email,
                FULLNAME = name,
                FACULTY_ID = faculty.Value,
                PASSWORD = password,
                PHONE = numberPhone,
                STATUS = false,
                MEMBER_TYPE = 0
            };
            db.CUSTOMERs.Add(cust);
            db.SaveChanges();
            ViewBag.SignInSucceed = true;
            return SignUp();
        }

        private bool CheckEmail(string email)
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

        public ActionResult UpdateCustomer()
        {
            if (Session["customerEmail"] == null || string.IsNullOrEmpty(Session["customerEmail"] as string))
            {
                return RedirectToAction("LogIn");
            }
            dynamic model = new ExpandoObject();
            string email = Session["customerEmail"].ToString();
            CUSTOMER cust = db.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email);
            model.CustomerInfo = cust;
            model.Faculties = db.FACULTies.ToList();
            return View(model);
        }

        [HttpPost]
        public ActionResult UpdateCustomer(string name, string password, string confirmPassword, string numberPhone, int? faculty)
        {
            if (Session["customerEmail"] == null || string.IsNullOrEmpty(Session["customerEmail"] as string))
            {
                return RedirectToAction("LogIn");
            }
            string email = Session["customerEmail"].ToString();
            var temp = db.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email);
            CUSTOMER customer = new CUSTOMER() { FACULTY_ID = faculty, FULLNAME = name, PASSWORD = password, EMAIL = email, PHONE = numberPhone };
            if (ValidateCustomerInfo(customer, confirmPassword))
            {
                temp.FACULTY_ID = customer.FACULTY_ID;
                temp.FULLNAME = customer.FULLNAME;
                temp.PASSWORD = customer.PASSWORD;
                temp.PHONE = customer.PHONE;
                db.SaveChanges();
            }
            ViewBag.UpdateCustomerInformationSucceed = true;
            return UpdateCustomer();
        }

        private bool ValidateCustomerInfo(CUSTOMER customer, string confirmPassword)
        {
            if (customer.FULLNAME.Trim() == "")
            {
                ModelState.AddModelError("emptyName", "Vui lòng nhập Họ và tên.");
                return false;
            }else if (customer.EMAIL.Trim() == "")
            {
                ModelState.AddModelError("emptyEmail", "Vui lòng nhập Email");
                return false;
            }else if (!CheckEmail(customer.EMAIL))
            {
                ModelState.AddModelError("invalidEmail", "Email không hợp lệ, vui lòng nhập lại");
                return false;
            }else if (customer.PASSWORD.Trim() == "")
            {
                ModelState.AddModelError("emptyPassword", "Vui lòng nhập mật khẩu");
                return false;
            }else if (confirmPassword == "")
            {
                ModelState.AddModelError("emptyConfirmPassword", "Vui lòng nhập mật khẩu xác nhận");
                return false;
            }else if (customer.PHONE == "")
            {
                ModelState.AddModelError("emptyPhone", "Vui lòng nhập số điện thoại");
                return false;
            }else if (!CheckPhone(customer.PHONE))
            {
                ModelState.AddModelError("invalidPhone", "Số điện thoại không hợp lệ, vui lòng nhập lại");
                return false;
            }
            else if (!customer.FACULTY_ID.HasValue)
            {
                ModelState.AddModelError("emptyFaculty", "Vui lòng chọn khoa");
                return false;
            }
            return true;
        }

        private bool CheckPhone(string phone)
        {
            Regex pattern = new Regex("^0[0-9]{9}$");
            return pattern.IsMatch(phone);
        }
    }
}