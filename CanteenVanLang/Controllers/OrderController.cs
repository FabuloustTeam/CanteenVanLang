using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;

namespace CanteenVanLang.Controllers
{
    public class OrderController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        private List<ORDER_DETAIL> cart = null;

        [CustomerVerification]
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

        [HttpPost]
        public JsonResult AddToCart(int idFood, int quantity)
        {
            var menu = getMenuToday();
            var choosenMenu = menu.Find(men => men.FOOD_ID == idFood);
            if (choosenMenu != null)
            {
                if (choosenMenu.QUANTITY >= quantity)
                {
                    var newItem = new ORDER_DETAIL();
                    newItem.MENU_ID = choosenMenu.ID;
                    newItem.MENU = choosenMenu;
                    newItem.QUANTITY = quantity;
                    newItem.UNIT_PRICE = choosenMenu.PRICE;

                    GetCart();
                    cart.Add(newItem);
                    var hashtable = new Hashtable();
                    foreach (var detail in cart)
                    {
                        if (hashtable[detail.MENU_ID] != null)
                        {
                            (hashtable[detail.MENU_ID] as ORDER_DETAIL).QUANTITY += detail.QUANTITY;
                        }
                        else
                        {
                            hashtable[detail.MENU_ID] = detail;
                        }
                    }
                    cart.Clear();
                    foreach (ORDER_DETAIL item in hashtable.Values)
                        cart.Add(item);

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, failQuantity = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, failQuantity = false }, JsonRequestBehavior.AllowGet);
        }

        private void GetCart()
        {
            if (Session["Cart"] != null)
                cart = Session["Cart"] as List<ORDER_DETAIL>;
            else
            {
                cart = new List<ORDER_DETAIL>();
                Session["Cart"] = cart;
            }
        }

        public ActionResult Cart()
        {
            GetCart();
            return View(cart);
        }

        [HttpPost]
        public JsonResult Remove(int id)
        {
            GetCart();
            var orderDetail = cart.Where(detail => detail.ID == id).FirstOrDefault();
            if (orderDetail != null)
            {
                cart.Remove(orderDetail);
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false }, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public JsonResult check(string a)
        {
            GetCart();
            var menu = getMenuToday();
            if (cart.Count > 0)
            {
                for (int i = 0; i < cart.Count; i++)
                {
                    var tmpMenu = menu.Where(men => men.ID == cart[i].MENU_ID).FirstOrDefault();
                    if (cart[i].QUANTITY > tmpMenu.QUANTITY)
                        return Json(new { success = false, name = cart[i].MENU.FOOD.FOOD_NAME }, JsonRequestBehavior.AllowGet);
                }
                return Json(new { success = true }, JsonRequestBehavior.AllowGet);
            }
            return Json(new { success = false, noItem = true }, JsonRequestBehavior.AllowGet);
        }

        [CustomerVerification]
        [HttpGet]
        public ActionResult CheckOut()
        {
            GetCart();
            return View(cart);
        }

        [CustomerVerification]
        public ActionResult PlaceOrder()
        {
            GetCart();
            if(cart.Count > 0)
            {
                var order = new ORDER();
                order.CUSTOMER_ID = (int)Session["customerId"];
                order.STATUS = 1;
                model.ORDERs.Add(order);
                model.SaveChanges();

                var listOrderDetails = new List<ORDER_DETAIL>();
                for (int i = 0; i < cart.Count; i++)
                {
                    listOrderDetails.Add(cart[i]);
                    var temp = new ORDER_DETAIL();
                    temp.ORDER_ID = order.ID;
                    temp.QUANTITY = cart[i].QUANTITY;
                    temp.UNIT_PRICE = cart[i].UNIT_PRICE;
                    temp.MENU_ID = cart[i].MENU_ID;
                    model.ORDER_DETAIL.Add(temp);
                    model.SaveChanges();
                }
                cart.Clear();
                var newOrder = new ORDER();
                newOrder.CUSTOMER = model.CUSTOMERs.Find(order.CUSTOMER_ID);
                newOrder.DATE = DateTime.Now;

                ViewBag.listOrderDetails = listOrderDetails;
                return View(newOrder);
            }
            return RedirectToAction("Cart");
        }

        private List<MENU> getMenuToday()
        {
            var today = DateTime.Now;
            var allMenu = model.MENUs.ToList();
            var menuToday = new List<MENU>();
            foreach (var item in allMenu)
            {
                if (item.DATE.Date == today.Date && item.STATUS == true)
                    menuToday.Add(item);
            }
            return menuToday;
        }

        public ActionResult BuyNow(int id)
        {
            GetCart();
            var menu = getMenuToday();
            var choosenMenu = menu.Find(men => men.FOOD_ID == id);
            var newItem = new ORDER_DETAIL();
            newItem.MENU_ID = choosenMenu.ID;
            newItem.MENU = choosenMenu;
            newItem.QUANTITY = 1;
            newItem.UNIT_PRICE = choosenMenu.PRICE;

            GetCart();
            cart.Add(newItem);
            var hashtable = new Hashtable();
            foreach (var detail in cart)
            {
                if (hashtable[detail.MENU_ID] != null)
                {
                    (hashtable[detail.MENU_ID] as ORDER_DETAIL).QUANTITY += detail.QUANTITY;
                }
                else
                {
                    hashtable[detail.MENU_ID] = detail;
                }
            }
            cart.Clear();
            foreach (ORDER_DETAIL item in hashtable.Values)
                cart.Add(item);
            return RedirectToAction("Cart");
        }

    }
}