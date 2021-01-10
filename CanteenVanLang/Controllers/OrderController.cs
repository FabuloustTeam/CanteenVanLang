using System;
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
        [CustomerVerification]
        // GET: Order
        public ActionResult Index()
        {
            return View();
        }

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
                    newItem.QUANTITY = quantity;
                    newItem.UNIT_PRICE = choosenMenu.PRICE;

                    if (Session["Cart"] != null)
                    {
                        List<ORDER_DETAIL> cart = Session["Cart"] as List<ORDER_DETAIL>;
                        cart.Add(newItem);
                        Session["Cart"] = cart;
                    }
                    else
                    {
                        List<ORDER_DETAIL> cart = new List<ORDER_DETAIL>();
                        cart.Add(newItem);
                        Session["Cart"] = cart;
                    }

                    return Json(new { success = true }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(new { success = false, failQuantity = true }, JsonRequestBehavior.AllowGet);
                }
            }
            return Json(new { success = false, failQuantity = false }, JsonRequestBehavior.AllowGet);
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