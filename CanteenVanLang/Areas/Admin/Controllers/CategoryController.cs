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
    public class CategoryController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();
        private const string PICTURE_PATH = "~/Images/Categories/";

        // GET: Admin/Category
        public ActionResult Index()
        {
            if ((int)Session["userRole"] == 3)
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            else
            {
                var category = model.CATEGORies.OrderByDescending(x => x.ID);
                return View(category);
            }
        }

        [HttpGet]
        public ActionResult Create()
        {
            if ((int)Session["userRole"] == 3)
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            else
            {
                ViewBag.Action = "Index";
                ViewBag.Controller = "Category";
                return View();
            }
        }
        [HttpPost]
        public ActionResult Create(CATEGORY newCategory, HttpPostedFileBase picture)
        {
            ValidateCategory(newCategory);
            ValidateImage(picture);
            if (ModelState.IsValid)
            {
                var category = new CATEGORY();
                //category.CATEGORY_CODE = newCategory.CATEGORY_CODE.Trim();
                category.CATEGORY_NAME = newCategory.CATEGORY_NAME.Trim();
                category.STATUS = false;
                model.CATEGORies.Add(category);
                model.SaveChanges();

                category.IMAGE_URL = @"\Images\Categories\" + category.ID + ".jpg";
                model.SaveChanges();

                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    picture.SaveAs(path + category.ID + ".jpg");

                }
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Index";
            ViewBag.Controller = "Category";
            return View(newCategory);
        }

        [HttpGet]
        public ActionResult Update(int id)
        {
            if ((int)Session["userRole"] == 3)
            {
                return RedirectToAction("Welcome", "Authentication");
            }
            else
            {
                var category = model.CATEGORies.FirstOrDefault(x => x.ID == id);
                ViewBag.Action = "Index";
                ViewBag.Controller = "Category";
                return View(category);
            }
        }

        [HttpPost]
        public ActionResult Update(int id, CATEGORY updatedCategory, HttpPostedFileBase picture)
        {
            ValidateCategory(updatedCategory);
            if (ModelState.IsValid)
            {
                var category = model.CATEGORies.FirstOrDefault(f => f.ID == id);
                category.CATEGORY_NAME = updatedCategory.CATEGORY_NAME.Trim();
                //category.CATEGORY_CODE = updatedCategory.CATEGORY_CODE.Trim();
                category.IMAGE_URL = @"\Images\Categories\" + id + ".jpg";
                category.STATUS = updatedCategory.STATUS;
                model.SaveChanges();
                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    picture.SaveAs(path + id + ".jpg");
                }
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Index";
            ViewBag.Controller = "Category";
            return View(updatedCategory);
        }

        private void ValidateCategory(CATEGORY model)
        {
            //if (model.CATEGORY_CODE.Trim() == "")
            //{
            //    ModelState.AddModelError("CATEGORY_CODE", "Vui lòng nhập mã loại món ăn");
            //}
            if (model.CATEGORY_NAME.Trim() == "")
            {
                ModelState.AddModelError("CATEGORY_NAME", "Vui lòng nhập tên loại món ăn");
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
                var category = model.CATEGORies.FirstOrDefault(c => c.ID == id);

                if (!isAbleToDelete(id))
                {
                    return Json(new { success = false, response = "cannotdelete" }, JsonRequestBehavior.AllowGet);
                }
                else
                {
                    model.CATEGORies.Remove(category);
                    model.SaveChanges();

                    var path = Server.MapPath(PICTURE_PATH);
                    System.IO.File.Delete(path + id + ".jpg");

                    scope.Complete();
                    return Json(new { success = true, response = "deleted" }, JsonRequestBehavior.AllowGet);
                }
            }
        }

        private bool isAbleToDelete(int idCate)
        {
            List<FOOD> allFoods = model.FOODs.ToList();
            for (int i = 0; i < allFoods.Count; i++)
            {
                if (allFoods[i].CATEGORY_ID == idCate)
                {
                    return false;
                }
            }
            return true;
        }
    }
}