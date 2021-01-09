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
                account.STATUS = updatedAccount.STATUS;
                account.ConfirmPassword = updatedAccount.ConfirmPassword.Trim();
                if (account.PASSWORD == account.ConfirmPassword)
                {
                    model.SaveChanges();
                }
                model.SaveChanges();

                //account.IMAGE_URL = @"\Images\Accounts\" + id + ".jpg";
                //model.SaveChanges();

                if (picture != null)
                {
                    var path = Server.MapPath(PICTURE_PATH);
                    System.IO.File.Delete(path + updatedAccount.ID + ".jpg");
                    picture.SaveAs(path + id + ".jpg");
                }

                Session["userFullName"] = account.FULLNAME;

                return RedirectToAction("Welcome", "Authentication");
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
        }
    }
}