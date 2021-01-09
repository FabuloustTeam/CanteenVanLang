using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using CanteenVanLang.Models;
using CanteenVanLang.Areas.Admin.Middleware;

namespace CanteenVanLang.Areas.Admin.Controllers
{
    [LoginVertification]
    public class AccountController : Controller
    {
        QUANLYCANTEENEntities model = new QUANLYCANTEENEntities();

        private const string PICTURE_PATH = "~/Images/Accounts/";

        // GET: Admin/Account
        public ActionResult Index()
        {
            if ((int)Session["userRole"] == 1)
            {
                var allAccounts = model.ACCOUNTs.OrderByDescending(acc => acc.ID).ToList();
                return View(allAccounts);
            }
            else
            {
                return RedirectToAction("Welcome", "Authentication");
            }
        }

        [HttpPost]
        public JsonResult Deactive(int id)
        {
            if(id != (int) Session["userId"])
            {
                var account = model.ACCOUNTs.FirstOrDefault(acc => acc.ID == id);
                account.STATUS = false;
                model.SaveChanges();
                return Json(new { success = true, url = Url.Action("Index", "Account") }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return Json(new { success = false, response = "deactive fail" }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public JsonResult Active(int id)
        {
            var account = model.ACCOUNTs.FirstOrDefault(acc => acc.ID == id);
            account.STATUS = true;
            model.SaveChanges();
            return Json(new { success = true, url = Url.Action("Index", "Account") }, JsonRequestBehavior.AllowGet);
        }
        
        [HttpGet]
        public ActionResult Create()
        {
            if ((int)Session["userRole"] == 1)
            {
                return View();
            }
            else
            {
                return RedirectToAction("Welcome", "Authentication");
            }
        }

        [HttpPost]
        public ActionResult Create(ACCOUNT newAccount, HttpPostedFileBase picture, string ConfirmPassword)
        {
            ValidateAccount(newAccount);
            if(ModelState.IsValid)
            {
                var account = new ACCOUNT();
                var namePicture = (newAccount.EMAIL.Trim()).Split('@')[0];

                account.EMAIL = newAccount.EMAIL.Trim();
                account.PASSWORD = newAccount.PASSWORD.Trim();
                account.FULLNAME = newAccount.FULLNAME.Trim();
                account.STATUS = newAccount.STATUS;
                account.ROLE = newAccount.ROLE;
                account.IMAGE_URL = @"\Images\Accounts\" + namePicture + ".jpg";
                model.ACCOUNTs.Add(account);
                model.SaveChanges();

                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    picture.SaveAs(path + namePicture + ".jpg");

                }
                return RedirectToAction("Index");
            }
            return View();
        }
        [HttpGet]
        public ActionResult Update (int id)
        {
            var account = model.ACCOUNTs.FirstOrDefault(f => f.ID == id);
            ViewBag.Action = "Index";
            ViewBag.Controller = "Account";
            return View(account);
        }

        [HttpPost]
        public ActionResult Update (int id, ACCOUNT updatedAccount, HttpPostedFileBase picture, string ConfirmPassword)
        {
            ValidateAccount(updatedAccount);
            if (ModelState.IsValid)
            {
                var account = model.ACCOUNTs.FirstOrDefault(f => f.ID == id);
                account.FULLNAME = updatedAccount.FULLNAME.Trim();
                account.PASSWORD = updatedAccount.PASSWORD.Trim();
                account.ConfirmPassword = updatedAccount.ConfirmPassword.Trim();
                if (account.PASSWORD == account.ConfirmPassword)
                {
                    model.SaveChanges();
                }
                model.SaveChanges();

                account.IMAGE_URL = @"\Images\Accounts\" + id + ".jpg";
                model.SaveChanges();

                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    System.IO.File.Delete(path + updatedAccount.ID + ".jpg");
                    picture.SaveAs(path + id + ".jpg");
                }
                return RedirectToAction("Index");
            }
            ViewBag.Action = "Index";
            ViewBag.Controller = "Account";
            return View(updatedAccount);
        }


        private void ValidateAccount(ACCOUNT model)
        {
            if (model.FULLNAME == null)
            {
                ModelState.AddModelError("FULLNAME", "Vui lòng nhập họ tên.");
            }
            if(model.ConfirmPassword.Trim() != model.PASSWORD.Trim())
            {
                    ModelState.AddModelError("ConfirmPassword", "Xác nhận mật khẩu không đúng");
            }
            if(model.ConfirmPassword.Trim() == "")
            {
                ModelState.AddModelError("ConfirmPassword", "Vui lòng nhập xác nhận mật khẩu");
            }
            if (model.PASSWORD.Trim() == "")
            {
                ModelState.AddModelError("PASSWORD", "Vui lòng nhập mật khẩu");
            }
            if(model.STATUS == false && model.ID == (int)Session["userId"])
            {
                ModelState.AddModelError("STATUS", "Không thể ngưng hoạt động tài khoản của chính bạn");
            }
        }
        private void ValidateImage(HttpPostedFileBase picture)
        {
            if (picture == null)
            {
                ModelState.AddModelError("IMAGE_URL", "Vui lòng thêm ảnh");
            }
        }
    }
}