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

        private List<MENU> getMenuToday()
        {
            var today = DateTime.Now;
            var allMenu = model.MENUs.ToList();
            var menuToday = new List<MENU>();
            foreach (var item in allMenu)
            {
                if (item.DATE.Date == today.Date)
                    menuToday.Add(item);
            }
            return menuToday;
        }

        [HttpGet]
        public ActionResult Create()
        {
            List<CUSTOMER> customers = model.CUSTOMERs.OrderByDescending(cust => cust.FULLNAME).ToList();
            List<MENU> menuToday = getMenuToday();
            ViewBag.customers = customers;
            ViewBag.menus = menuToday;
            ViewBag.IsNoMenu = false;
            if (menuToday.Count == 0)
            {
                ViewBag.IsNoMenu = true;
            }
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
        public JsonResult getPrice(string menuCode)
        {
            int price = model.MENUs.FirstOrDefault(menu => menu.MENU_CODE == menuCode).PRICE;
            return Json(new { success = true, value = price }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult checkQuantity(int quantity, int idMenu)
        {
            var menu = model.MENUs.FirstOrDefault(men => men.ID == idMenu);
            if(menu.QUANTITY >= quantity)
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            else
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }
    }
}