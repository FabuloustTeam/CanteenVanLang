using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;

namespace CanteenVanLang.Controllers
{
    [HandleError]
    public class HomeController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        public ActionResult Index()
        {
            ViewBag.Categories = model.CATEGORies.Where(cate => cate.FOODs.Count > 0).ToList();
            var today = DateTime.Now;
            var menu = model.MENUs.ToList();
            var foodToday = new List<FOOD>();
            foreach(var item in menu)
            {
                if(item.DATE.Date == today.Date)
                {
                    foodToday.Add(item.FOOD);
                }
            }
            if(foodToday.Count > 0)
            {
                ViewBag.Menu = foodToday;
            }
            else
            {
                ViewBag.Menu = model.FOODs.ToList();
            }

            return View();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}