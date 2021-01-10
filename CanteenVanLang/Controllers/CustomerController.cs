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
        private QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        // GET: Customer/Auth
        public ActionResult SignUp()
        {
            return View(model.FACULTies.ToList());
        }

        [HttpPost]
        public ActionResult SignUp(CUSTOMER newCustomer)
        {
            ValidateCustomerInfo(newCustomer);  
            if (ModelState.IsValid)
            {  
                CUSTOMER cust = new CUSTOMER
                {
                    EMAIL = newCustomer.EMAIL,
                    FULLNAME = newCustomer.FULLNAME,
                    FACULTY_ID = newCustomer.FACULTY_ID,
                    PASSWORD = newCustomer.PASSWORD,
                    PHONE = newCustomer.PHONE,
                    STATUS = false,
                    MEMBER_TYPE = 0      
                };
                model.SaveChanges();
                return RedirectToAction("LogIn");
            }
            ViewBag.SignInSucceed = true;
            return View();
        }

        public ActionResult LogIn()
        {
            if (Session["customerName"] != null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                Session["incorrectPwd"] = false;
                Session["customerNotFound"] = false;
                return View();
            }
        }

        [HttpPost]
        public ActionResult LogIn(string email, string password)
        {
            Session["incorrectPwd"] = false;
            Session["customerNotFound"] = false;
            ValidateLogIn(email, password);
            if (ModelState.IsValid)
            {
                var customer = model.CUSTOMERs.FirstOrDefault(cus => cus.EMAIL.Equals(email));
                if (customer != null)
                {
                    if (customer.PASSWORD.Equals(password))
                    {
                        Session["customerName"] = customer.FULLNAME;
                        Session["customerEmail"] = customer.EMAIL;
                        return RedirectToAction("Index", "Home");
                    }
                    else
                    {
                        Session["incorrectPwd"] = true;
                        ViewBag.email = customer.EMAIL;
                        return View();
                    }
                }
                Session["customerNotFound"] = true;
                return View();
            }
            return View();
        }

        public ActionResult Update()
        {
            if (Session["customerEmail"] == null || string.IsNullOrEmpty(Session["customerEmail"] as string))
            {
                return RedirectToAction("LogIn");
            }
            //dynamic model = new ExpandoObject();
            string email = Session["customerEmail"].ToString();
            CUSTOMER cust = this.model.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email);
            ViewBag.Categories = model.FACULTies.OrderByDescending(x => x.ID).ToList();
            ViewBag.Action = "Index";
            ViewBag.Controller = "Customer";
            return View(cust);
        }

        [HttpPost]
        public ActionResult Update(CUSTOMER updateCustomer)
        {
            if (Session["customerEmail"] == null || string.IsNullOrEmpty(Session["customerEmail"] as string))
            {
                return RedirectToAction("LogIn");
            }
            string email = Session["customerEmail"].ToString();

            //CUSTOMER customer = new CUSTOMER() { FACULTY_ID = faculty, FULLNAME = name, PASSWORD = password, EMAIL = email, PHONE = numberPhone };
            ValidateCustomerInfo(updateCustomer);
            if (ModelState.IsValid)
            {
                var customer = model.CUSTOMERs.FirstOrDefault(item => item.EMAIL == email);
                customer.FACULTY_ID = updateCustomer.FACULTY_ID;
                customer.FULLNAME = updateCustomer.FULLNAME;
                customer.PASSWORD = updateCustomer.PASSWORD;
                customer.PHONE = updateCustomer.PHONE;
                customer.confirmPassword = updateCustomer.confirmPassword;
                if (updateCustomer.PASSWORD == updateCustomer.confirmPassword)
                {
                    model.SaveChanges();
                }
                model.SaveChanges();
                Session["customerName"] = customer.FULLNAME;
            }
            ViewBag.Action = "Index";
            ViewBag.Controller = "Customer";
            ViewBag.UpdateCustomerInformationSucceed = true;
            return View();
        }

        public ActionResult Logout()
        {
            Session["customerName"] = null;
            Session["customerEmail"] = null;
            return RedirectToAction("Index", "Home");
        }
        private void ValidateLogIn( string email, string password)
        {
            if (email.Trim() == "")
            {
                ModelState.AddModelError("EMAIL", "Vui lòng nhập Email");
            }
            if (password.Trim() == "")
            {
                ModelState.AddModelError("PASSWORD", "Vui lòng nhập mật khẩu");
            }
        }

        private void ValidateCustomerInfo(CUSTOMER customer)
        {
            if (customer.FULLNAME.Trim() == "")
            {
                ModelState.AddModelError("FULLNAME", "Vui lòng nhập Họ và tên.");
            }
            if (customer.EMAIL.Trim() == "")
            {
                ModelState.AddModelError("EMAIL", "Vui lòng nhập Email");
            }
            else if (model.CUSTOMERs.Count(item => item.EMAIL == customer.EMAIL) == 1)
            {
                ModelState.AddModelError("EMAIL", "Email đã tồn tại");
            }
            else if (!CheckEmail(customer.EMAIL))
            {
                ModelState.AddModelError("EMAIL", "Email không hợp lệ, vui lòng nhập lại");
            }
            if (customer.PASSWORD.Trim() == "")
            {
                ModelState.AddModelError("PASSWORD", "Vui lòng nhập mật khẩu");
            }
            if ( customer.confirmPassword.Trim() == "")
            {
                ModelState.AddModelError("PASSWORD", "Vui lòng nhập mật khẩu xác nhận");
            }
            if (customer.PHONE.Trim() == "")
            {
                ModelState.AddModelError("PHONE", "Vui lòng nhập số điện thoại");
            }
            else if (!CheckPhone(customer.PHONE))
            {
                ModelState.AddModelError("PHONE", "Số điện thoại không hợp lệ, vui lòng nhập lại");
            }
            //if (!customer.FACULTY_ID.HasValue)
            //{
            //    ModelState.AddModelError("emptyFaculty", "Vui lòng chọn khoa");
            //}
        }

        private bool CheckPhone(string phone)
        {
            Regex pattern = new Regex("^0[0-9]{9}$");
            return pattern.IsMatch(phone);
        }
        private bool CheckEmail(string email)
        {
            Regex regex = new Regex("/^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9-]+(?:.[a-zA-Z0-9-]+)*$/");
            return regex.IsMatch(email);
        }
    }
}