using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;


namespace CanteenVanLang.Controllers
{
    public class FoodController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        // GET: Food
        public ActionResult Index()
        {
            var listFoods = model.FOODs.OrderByDescending(x => x.ID).ToList();
            return View(listFoods);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(400, "Bad Request");
            }
            FOOD food = model.FOODs.Find(id);
            if (food == null)
            {
                return HttpNotFound("Not Found");
            }
            ViewData["list"] = model.FOODs.Where(x => x.ID == id).ToList<FOOD>();
            return View(food);
        }
    }
}