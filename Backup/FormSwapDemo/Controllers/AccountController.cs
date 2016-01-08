using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using FormSwapDemo.Models;

namespace FormSwapDemo.Controllers
{
    [Authorize]
    public class AccountController : Controller
    {
        //
        // GET: /Account/Login

        [AllowAnonymous]
        public PartialViewResult _Login()
        {            
            return PartialView();
        }

        //
        // POST: /Account/Login

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult _Login(LoginModel model)
        {
            ///
            /// NOTE: This is where your logic goes
            /// 

            //for testing           
            if (model.UserName == "fail")
            {
                return Json(JsonResponseFactory.ErrorResponse("You told me to fail."), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.SuccessResponse(), JsonRequestBehavior.DenyGet);
            }
        }

        //
        // GET: /Account/Register

        [AllowAnonymous]
        public PartialViewResult _Register()
        {
            return PartialView();
        }

        //
        // POST: /Account/Register

        [HttpPost]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public JsonResult _Register(RegisterModel model)
        {
            //for testing           
            if (model.UserName == "fail")
            {
                return Json(JsonResponseFactory.ErrorResponse("You told me to fail."), JsonRequestBehavior.DenyGet);
            }
            else
            {
                return Json(JsonResponseFactory.SuccessResponse(), JsonRequestBehavior.DenyGet);
            }   
        }
    }
}
