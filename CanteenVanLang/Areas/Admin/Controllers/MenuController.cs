using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Areas.Admin.Middleware;
using CanteenVanLang.Models;

namespace CanteenVanLang.Areas.Admin.Controllers
{
    [LoginVertification]
    public class MenuController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        // GET: Admin/Menu
        public ActionResult Index()
        {
            var menu = model.MENUs.OrderByDescending(men => men.DATE).ToList();
            return View(menu);
        }

        [HttpGet]
        public ActionResult Create()
        {
            ViewBag.Foods = model.FOODs.OrderBy(food => food.FOOD_NAME).ToList();
            return View();
        }

        [HttpPost]
        public ActionResult Create(string date, string FOOD_ID, string quantity, string price, string status)
        {
            Validate(date, FOOD_ID, quantity, price);
            if (ModelState.IsValid)
            {
                var newMenu = new MENU();
                newMenu.DATE = DateTime.Parse(date);
                newMenu.FOOD_ID = Int32.Parse(FOOD_ID);
                newMenu.QUANTITY = Int32.Parse(quantity);
                newMenu.PRICE = Int32.Parse(price);
                newMenu.STATUS = Boolean.Parse(status);
                ValidateMenu(newMenu);
                if (ModelState.IsValid)
                {
                    var menu = new MENU();
                    menu.DATE = newMenu.DATE;
                    menu.ACCOUNT_ID = (int)Session["userId"];

                    menu.FOOD_ID = newMenu.FOOD_ID;
                    menu.QUANTITY = newMenu.QUANTITY;
                    menu.PRICE = newMenu.PRICE;
                    menu.STATUS = newMenu.STATUS;

                    model.MENUs.Add(menu);
                    model.SaveChanges();

                    return RedirectToAction("Index");
                }
                return View(newMenu);
            }
            ViewBag.Foods = model.FOODs.OrderBy(food => food.FOOD_NAME).ToList();
            return View();
        }

        private void Validate(string date, string foodId, string quantity, string price)
        {
            DateTime dt;
            if(!DateTime.TryParse(date, out dt))
            {
                ModelState.AddModelError("DATE", "Vui lòng chọn ngày");
            }
            if(foodId == null)
            {
                ModelState.AddModelError("FOOD_ID", "Vui lòng chọn món ăn");
            }
            if (quantity == null)
            {
                ModelState.AddModelError("QUANTITY", "Vui lòng nhập số lượng");
            }
            if (price == null)
            {
                ModelState.AddModelError("PRICE", "Vui lòng nhập giá");
            }
        }

        private void ValidateMenu(MENU menu)
        {
            if (menu.FOOD_ID == null)
            {
                ModelState.AddModelError("FOOD_ID", "Vui lòng chọn món ăn");
            }
        }

        [HttpPost]
        public JsonResult getPrice(string idReceived)
        {
            int id = Int32.Parse(idReceived);
            int price = model.FOODs.FirstOrDefault(food => food.ID == id).PRICE;
            return Json(new { success = true, value = price }, JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var menu = model.MENUs.FirstOrDefault(men => men.ID == id);
            ViewBag.Foods = model.FOODs.OrderBy(food => food.FOOD_NAME).ToList();
            return View(menu);
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var scope = new TransactionScope())
            {
                var menu = model.MENUs.FirstOrDefault(f => f.ID == id);

                if (menu.STATUS || checkIfMenuInAnyOrder(id))
                {
                    return Json(new { success = false, response = "cannotdelete" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.MENUs.Remove(menu);
                    model.SaveChanges();
                    
                    scope.Complete();
                    return Json(new { success = true, response = "deleted" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private bool checkIfMenuInAnyOrder(int id)
        {
            if (model.ORDER_DETAIL.Where(ordDetail => ordDetail.MENU_ID == id) != null)
                return true;
            else
                return false;
        }
    }
}