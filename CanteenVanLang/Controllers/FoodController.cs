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
    [HandleError]
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
            ViewBag.Categories = model.CATEGORies.Where(cate => cate.FOODs.Count > 0).ToList();
            return View(listFoods);
        }

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(400, "Bad Request");
            }
            var listFoods = getFoods();
            FOOD food = listFoods.FirstOrDefault(f => f.ID == id);
            if (food == null)
            {
                return HttpNotFound("Not Found");
            }
            ViewBag.NameFood = food.FOOD_NAME;
            ViewData["list"] = model.FOODs.Where(x => x.ID == id).ToList<FOOD>();
            return View(food);
        }

        public ActionResult Search(string searching)
        {
            var listFoods = getFoods();
            var searchResults = new List<FOOD>();
            foreach (var item in listFoods)
            {
                var name = ConvertToUnSign(item.FOOD_NAME).ToLower();
                var category = ConvertToUnSign(item.CATEGORY.CATEGORY_NAME).ToLower();
                var description = ConvertToUnSign(item.DESCRIPTION).ToLower();
                var searchingParsed = ConvertToUnSign(searching);
                if (name.Contains(searchingParsed) || category.Contains(searchingParsed) || description.Contains(searchingParsed)
                     || item.PRICE.ToString().Contains(searching))
                {
                    searchResults.Add(item);
                }
            }
            ViewBag.Categories = model.CATEGORies.Where(cate => cate.FOODs.Count > 0).ToList();
            ViewBag.Searching = searching;
            ViewBag.Count = searchResults.Count;
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

        public ActionResult Category(int id)
        {
            var listFoods = getFoods().Where(food => food.CATEGORY_ID == id).ToList<FOOD>();
            ViewBag.Categories = model.CATEGORies.Where(cate => cate.FOODs.Count > 0).ToList();
            ViewBag.Category = model.CATEGORies.FirstOrDefault(cate => cate.ID == id);
            return View(listFoods);
        }
    }
}