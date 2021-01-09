using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;


namespace CanteenVanLang.Controllers
{
    public class FoodController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();

        private List<FOOD> getFoods()
        {
            var listFoods = model.FOODs.OrderBy(f => f.FOOD_NAME).ToList();
            var listIdFood = getIdFoodInTodayMenu();
            for (int i = 0; i < listFoods.Count; i++)
            {
                if (listIdFood.Contains(listFoods[i].ID))
                    listFoods[i].STATUS = true;
                else
                    listFoods[i].STATUS = false;
            }
            return listFoods;
        }
        
        private List<int> getIdFoodInTodayMenu()
        {
            var menuToday = getMenuToday();
            var listIdFood = new List<int>();
            foreach(var item in menuToday)
            {
                listIdFood.Add(item.FOOD_ID);
            }
            return listIdFood;
        }

        private List<MENU> getMenuToday()
        {
            var today = DateTime.Now;
            var allMenu = model.MENUs.ToList();
            var menuToday = new List<MENU>();
            foreach(var item in allMenu)
            {
                if (item.DATE.Date == today.Date)
                    menuToday.Add(item);
            }
            return menuToday;
        }

        public ActionResult Index()
        {
            var listFoods = getFoods();
            ViewBag.Categories = model.CATEGORies.ToList();
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

        [HttpPost]
        public ActionResult Search(string searching)
        {
            var listFoods = getFoods();
            var searchResults = new List<FOOD>();
            foreach (var item in listFoods)
            {
                if (ConvertToUnSign(item.FOOD_NAME).ToLower().Contains((ConvertToUnSign(searching))))
                {
                    searchResults.Add(item);
                }
            }
            ViewBag.Categories = model.CATEGORies.ToList();
            return View(searchResults);
        }

        private string ConvertToUnSign(string input)
        {
            input = input.Trim();
            for (int i = 0x20; i < 0x30; i++)
            {
                input = input.Replace(((char)i).ToString(), " ");
            }
            Regex regex = new Regex(@"\p{IsCombiningDiacriticalMarks}+");
            string str = input.Normalize(NormalizationForm.FormD);
            string str2 = regex.Replace(str, string.Empty).Replace('đ', 'd').Replace('Đ', 'D');
            while (str2.IndexOf("?") >= 0)
            {
                str2 = str2.Remove(str2.IndexOf("?"), 1);
            }
            return str2;
        }
    }
}