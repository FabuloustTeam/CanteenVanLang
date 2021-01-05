using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;
using CanteenVanLang.Areas.Admin.Middleware;

namespace CanteenVanLang.Areas.Admin.Controllers
{
    [LoginVertification]
    public class OrderController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();

        // GET: Admin/Order
        public ActionResult Index()
        {
            var allOrders = model.ORDERs.OrderByDescending(ord => ord.DATE).ToList();
            return View(allOrders);
        }

        public ActionResult Print(int id)
        {
            var printData = model.ORDERs.FirstOrDefault(ord => ord.ID == id);
            return View(printData);
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<CUSTOMER> customers = model.CUSTOMERs.OrderByDescending(cust => cust.FULLNAME).ToList();
            List<FOOD> foods = model.FOODs.OrderByDescending(food => food.FOOD_NAME).ToList();
            ViewBag.customers = customers;
            ViewBag.foods = foods;
            return View();
        }

        [HttpPost]
        public JsonResult Create(List<ORDER_DETAIL> orderDetails, int customerID)
        {
            var order = new ORDER();
            order.ACCOUNT_ID = (int)Session["userId"];
            order.CUSTOMER_ID = customerID;
            order.STATUS = 1;

            foreach(var detail in orderDetails)
            {
                model.ORDER_DETAIL.Add(detail);
            }

            model.ORDERs.Add(order);
            model.SaveChanges();
            return Json(new { success = true }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult getPrice(string foodCode)
        {
            int price = model.FOODs.FirstOrDefault(food => food.FOOD_CODE == foodCode).PRICE;
            return Json(new { success = true, value = price }, JsonRequestBehavior.AllowGet);
        }
    }
}