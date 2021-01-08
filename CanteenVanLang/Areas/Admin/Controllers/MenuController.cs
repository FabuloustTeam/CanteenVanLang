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

        //[HttpPost]
        //public JsonResult Create(MENU newMenu)
        //{
            
        //}

        [HttpPost]
        public JsonResult getPrice(string foodCode)
        {
            int price = model.FOODs.FirstOrDefault(food => food.FOOD_CODE == foodCode).PRICE;
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