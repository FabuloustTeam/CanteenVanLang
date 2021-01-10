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
            if(choosenMenu != null)
            {
                if(choosenMenu.QUANTITY >= quantity)
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

    }
}