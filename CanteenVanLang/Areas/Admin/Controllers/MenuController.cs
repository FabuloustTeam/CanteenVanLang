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

        //[HttpPost]
        //public ActionResult Create(MENU newMenu)
        //{

        //}
    }
}