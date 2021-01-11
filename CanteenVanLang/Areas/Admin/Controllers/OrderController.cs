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
            if((bool) Session["newOrder"])
            {
                var allOrder = model.ORDERs.OrderByDescending(ord => ord.ID).ToList();
                var newOrder = allOrder[allOrder.Count - 1];
                allOrder.Remove(newOrder);
                allOrder.OrderBy(ord => ord.DATE).ToList();
                allOrder.Add(newOrder);
                Session["newOrder"] = false;
                return View(allOrder);
            }
            else
            {
                var allOrders = model.ORDERs.OrderByDescending(ord => ord.DATE).ToList();
                return View(allOrders);
            }
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
        public JsonResult Create(List<ORDER_DETAIL> orderDetails, string customerID)
        {
            if(customerID == null || customerID.Equals("none"))
            {
                return Json(new { success = false, caseFalse = "no customer" }, JsonRequestBehavior.AllowGet);
            }
            else if(orderDetails.Count == 0)
            {
                return Json(new { success = false, caseFalse = "no details" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var order = new ORDER();
                order.ACCOUNT_ID = (int)Session["userId"];
                order.CUSTOMER_ID = Int32.Parse(customerID);
                order.STATUS = 1;
                model.ORDERs.Add(order);
                model.SaveChanges();

                foreach (var detail in orderDetails)
                {
                    detail.ORDER_ID = order.ID;
                    model.ORDER_DETAIL.Add(detail);
                    model.SaveChanges();
                }
                Session["newOrder"] = true;
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var order = model.ORDERs.FirstOrDefault(ord => ord.ID == id);
            var listOrderDetails = model.ORDER_DETAIL.Where(detail => detail.ORDER_ID == id).ToList<ORDER_DETAIL>();
            var menus = getMenuOnDate((DateTime) order.DATE);
            ViewBag.listOrderDetails = listOrderDetails;
            ViewBag.menus = menus;
            if (menus.Count == 0)
            {
                ViewBag.IsNoMenu = true;
            }
            return View(order);
        }

        [HttpPost]
        public JsonResult Update(int id, List<ORDER_DETAIL> orderDetails, ORDER order)
        {
            if (orderDetails.Count == 0)
            {
                return Json(new { success = false, caseFalse = "no details" }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                var oldListOrderDetails = model.ORDER_DETAIL.Where(detail => detail.ORDER_ID == id).ToList();
                for(int i = 0; i < oldListOrderDetails.Count; i++)
                {
                    model.ORDER_DETAIL.Remove(oldListOrderDetails[i]);
                    model.SaveChanges();
                }

                foreach (var detail in orderDetails)
                {
                    detail.ORDER_ID = id;
                    model.ORDER_DETAIL.Add(detail);
                    model.SaveChanges();
                }

                var updatedOrder = model.ORDERs.FirstOrDefault(ord => ord.ID == id);
                updatedOrder.STATUS = order.STATUS;
                model.SaveChanges();

                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpGet]
        public ActionResult Details(int id)
        {
            var order = model.ORDERs.FirstOrDefault(ord => ord.ID == id);
            var listOrderDetails = model.ORDER_DETAIL.Where(detail => detail.ORDER_ID == id).ToList<ORDER_DETAIL>();
            ViewBag.listOrderDetails = listOrderDetails;
            return View(order);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            var order = model.ORDERs.FirstOrDefault(ord => ord.ID == id);
            if(order.STATUS == 2)
            {
                return Json(new { success = false }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                order.STATUS = 4;
                model.SaveChanges();
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
        }

        private List<MENU> getMenuOnDate(DateTime date)
        {
            var allMenus = model.MENUs.ToList();
            var menus = new List<MENU>();
            foreach(var item in allMenus)
            {
                if (item.DATE.Date == date.Date)
                    menus.Add(item);
            }
            return menus;
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