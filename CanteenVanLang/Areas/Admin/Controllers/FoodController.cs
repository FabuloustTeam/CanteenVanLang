using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;
using System.Transactions;
using CanteenVanLang.Areas.Admin.Middleware;

namespace CanteenVanLang.Areas.Admin.Controllers
{
    [LoginVertification]
    [PermissionVertification]
    public class FoodController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();

        private const string PICTURE_PATH = "~/Images/Foods/";

        // GET: Admin/Food
        public ActionResult Index()
        {

            if ((bool)Session["newFood"] == true)
            {
                var allFoods = model.FOODs.OrderByDescending(food => food.ID).ToList();
                var newFood = allFoods[allFoods.Count - 1];
                allFoods.Remove(newFood);
                allFoods.OrderBy(food => food.FOOD_NAME).ToList();
                allFoods.Add(newFood);
                Session["newFood"] = false;
                return View(allFoods);
            }
            else
            {
                var allFoods = model.FOODs.OrderBy(food => food.FOOD_NAME).ToList();
                return View(allFoods);
            }

        }

        [HttpGet]
        public ActionResult Create()
        {

            ViewBag.Categories = model.CATEGORies.OrderByDescending(x => x.ID).ToList();
            ViewBag.Action = "Index";
            ViewBag.Controller = "Food";
            return View();

        }

        [HttpPost]
        public ActionResult Create(FOOD newFood, HttpPostedFileBase picture)
        {
            ValidateFood(newFood);
            ValidateImage(picture);
            if (ModelState.IsValid)
            {
                var food = new FOOD();
                food.FOOD_NAME = newFood.FOOD_NAME.Trim();
                food.DESCRIPTION = newFood.DESCRIPTION;
                if (newFood.DESCRIPTION != null)
                {
                    food.DESCRIPTION = newFood.DESCRIPTION.Trim();
                }
                food.PRICE = newFood.PRICE;
                food.CATEGORY_ID = newFood.CATEGORY_ID;
                food.STATUS = newFood.STATUS;
                model.FOODs.Add(food);
                model.SaveChanges();

                food.IMAGE_URL = @"\Images\Foods\" + food.ID + ".jpg";
                model.SaveChanges();

                changeCategoryStatusForCreating((int)food.CATEGORY_ID, food.STATUS);

                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    picture.SaveAs(path + food.ID + ".jpg");

                }
                Session["newFood"] = true;
                return RedirectToAction("Index");
            }
            ViewBag.Categories = model.CATEGORies.OrderByDescending(x => x.ID).ToList();
            ViewBag.Action = "Index";
            ViewBag.Controller = "Food";
            return View(newFood);
        }

        private void changeCategoryStatusForCreating(int idCategory, bool foodStatus)
        {
            if (foodStatus)
            {
                CATEGORY category = model.CATEGORies.FirstOrDefault(cate => cate.ID == idCategory);
                category.STATUS = true;
                model.SaveChanges();
            }
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            var food = model.FOODs.FirstOrDefault(f => f.ID == id);
            ViewBag.Categories = model.CATEGORies.OrderByDescending(x => x.ID).ToList();
            ViewBag.Action = "Index";
            ViewBag.Controller = "Food";
            return View(food);
        }

        [HttpPost]
        public ActionResult Update(int id, FOOD updatedFood, HttpPostedFileBase picture, string idCategory)
        {
            ValidateFood(updatedFood);
            if (ModelState.IsValid)
            {
                using (var scope = new TransactionScope())
                {
                    var food = model.FOODs.FirstOrDefault(f => f.ID == id);
                    food.FOOD_NAME = updatedFood.FOOD_NAME.Trim();
                    food.DESCRIPTION = updatedFood.DESCRIPTION.Trim();
                    if (updatedFood.DESCRIPTION != null)
                    {
                        food.DESCRIPTION = updatedFood.DESCRIPTION.Trim();
                    }
                    food.PRICE = updatedFood.PRICE;
                    food.CATEGORY_ID = updatedFood.CATEGORY_ID;
                    food.STATUS = updatedFood.STATUS;
                    model.SaveChanges();

                    food.IMAGE_URL = @"\Images\Foods\" + updatedFood.ID + ".jpg";
                    model.SaveChanges();

                    int idOldCategory = Int32.Parse(idCategory);
                    changeCategoryStatusForUpdating((int)updatedFood.CATEGORY_ID, idOldCategory, food.ID, food.STATUS);

                    if (picture != null)
                    {
                        var path = Server.MapPath(PICTURE_PATH);
                        System.IO.File.Delete(path + updatedFood.ID + ".jpg");
                        picture.SaveAs(path + updatedFood.ID + ".jpg");
                    }
                    scope.Complete();
                    return RedirectToAction("Index");
                }
            }
            ViewBag.Categories = model.CATEGORies.OrderByDescending(x => x.ID).ToList();
            ViewBag.Action = "Index";
            ViewBag.Controller = "Food";
            return View(updatedFood);
        }

        private void changeCategoryStatusForUpdating(int idNewCate, int idOldCate, int idFood, bool statusFood)
        {
            if (statusFood)
            {
                if (idNewCate != idOldCate)
                {
                    CATEGORY newCate = model.CATEGORies.FirstOrDefault(cate => cate.ID == idNewCate);
                    newCate.STATUS = true;

                    CATEGORY oldCate = model.CATEGORies.FirstOrDefault(cate => cate.ID == idOldCate);
                    oldCate.STATUS = checkCategoryStatus(idOldCate, idFood);

                    model.SaveChanges();
                }
                else
                {
                    var category = model.CATEGORies.FirstOrDefault(cate => cate.ID == idNewCate);
                    category.STATUS = true;
                    model.SaveChanges();
                }
            }
            else
            {
                if (idNewCate != idOldCate)
                {
                    CATEGORY newCate = model.CATEGORies.FirstOrDefault(cate => cate.ID == idNewCate);
                    newCate.STATUS = checkCategoryStatus(idNewCate, idFood);

                    CATEGORY oldCate = model.CATEGORies.FirstOrDefault(cate => cate.ID == idOldCate);
                    oldCate.STATUS = checkCategoryStatus(idOldCate, idFood);

                    model.SaveChanges();
                }
                else
                {
                    CATEGORY category = model.CATEGORies.FirstOrDefault(cate => cate.ID == idOldCate);
                    category.STATUS = checkCategoryStatus(idOldCate, idFood);

                    model.SaveChanges();
                }
            }

        }

        private bool checkCategoryStatus(int idCate, int idExceptFood)
        {
            List<FOOD> allFoods = model.FOODs.ToList();
            for (int i = 0; i < allFoods.Count; i++)
            {
                if (allFoods[i].ID != idExceptFood)
                {
                    if (allFoods[i].CATEGORY_ID == idCate && allFoods[i].STATUS)
                    {
                        return true;
                    }
                }
            }
            return false;
        }

        private void ValidateFood(FOOD model)
        {
            if (model.PRICE < 0)
            {
                ModelState.AddModelError("PRICE", "Giá không hợp lệ");
            }
            if(model.FOOD_NAME == null)
            {
                ModelState.AddModelError("FOOD_NAME", "Vui lòng nhập tên món ăn");
            }
            else
            {
                if (model.FOOD_NAME.Trim() == "")
                {
                    ModelState.AddModelError("FOOD_NAME", "Vui lòng nhập tên món ăn");
                }
            }

            if(model.DESCRIPTION == null)
            {
                ModelState.AddModelError("DESCRIPTION", "Vui lòng nhập mô tả");
            }
            else
            {
                if (model.DESCRIPTION.Trim() == "")
                {
                    ModelState.AddModelError("DESCRIPTION", "Vui lòng nhập mô tả");
                }
            }
        }

        private void ValidateImage(HttpPostedFileBase picture)
        {
            if (picture == null)
            {
                ModelState.AddModelError("IMAGE_URL", "Vui lòng thêm ảnh");
            }
        }

        [HttpPost]
        public JsonResult Delete(int id)
        {
            using (var scope = new TransactionScope())
            {
                var food = model.FOODs.FirstOrDefault(f => f.ID == id);

                if (checkIfNotAbleDelete(id))
                {
                    return Json(new { success = false, response = "cannotdelete" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.FOODs.Remove(food);
                    model.SaveChanges();

                    var path = Server.MapPath(PICTURE_PATH);
                    System.IO.File.Delete(path + id + ".jpg");

                    scope.Complete();
                    return Json(new { success = true, response = "deleted" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private bool checkIfNotAbleDelete(int id)
        {
            if (model.MENUs.Where(menu => menu.FOOD_ID == id).ToList().Count > 0)
                return true;
            return false;
        }
    }
}