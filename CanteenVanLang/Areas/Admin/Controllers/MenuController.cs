using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;

namespace CanteenVanLang.Areas.Admin.Controllers
{
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
        public JsonResult getPrice(string foodCode)
        {
            int price = model.FOODs.FirstOrDefault(food => food.FOOD_CODE == foodCode).PRICE;
            return Json(new { success = true, value = price }, JsonRequestBehavior.AllowGet);
        }

        //[HttpPost]
        //public ActionResult Create(MENU newMenu)
        //{

        //}

        [HttpGet]
        public ActionResult Update(int id)
        {
            var menu = model.MENUs.FirstOrDefault(men => men.ID == id);
            ViewBag.Foods = model.FOODs.OrderBy(food => food.FOOD_NAME).ToList();
            return View(menu);
        }
    }
}