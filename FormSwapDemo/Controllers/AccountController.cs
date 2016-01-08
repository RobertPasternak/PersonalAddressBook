using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Transactions;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using DotNetOpenAuth.AspNet;
using Microsoft.Web.WebPages.OAuth;
using WebMatrix.WebData;
using FormSwapDemo.Models;
using FormSwapDemo.Utils;

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
        public JsonResult _Login(LoginModel loginModel)
        {
            ///
            /// NOTE: This is where your logic goes
            /// 

            //for testing           
            if (ModelState.IsValid)
            {
                if (loginModel.IsValidUser(loginModel.Login, loginModel.Password))
                {
                    Session["user"] = new LoginModel() {Login = loginModel.Login};
                    return Json(JsonResponseFactory.SuccessResponse(), JsonRequestBehavior.DenyGet);
                }
                else
                {
                    ModelState.AddModelError("", "Błędne dane logowania.");
                    return Json(JsonResponseFactory.ErrorResponse("Błędne dane logowania."), JsonRequestBehavior.DenyGet);
                }
                
                

            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse(""), JsonRequestBehavior.DenyGet);
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
        public JsonResult _Register(RegisterModel registerModel)
        {
            if (ModelState.IsValid)
            {
                var conn =
                    new SqlConnection(
                        "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

                var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l ") { Connection = conn };
                cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = registerModel.Login;
                conn.Open();
                var reader = cmd.ExecuteReader();


                if (reader.HasRows)
                {
                    conn.Close();
                    return Json(JsonResponseFactory.ErrorResponse("Wybrana nazwa użytkownika jest już zajęta."), JsonRequestBehavior.DenyGet);
                }
                else
                {


                    var addCmd =
                        new SqlCommand(
                            "INSERT INTO [Users] ([Login], [Password], [LoginAttempts]) VALUES (@login, @password, @attempts)")
                        {
                            Connection = conn
                        };
                    addCmd.Parameters.Clear();

                    addCmd.Parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar)).Value = registerModel.Login;
                    addCmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value =
                        registerModel.Password;
                    addCmd.Parameters.Add(new SqlParameter("@attempts", SqlDbType.Int)).Value = 0;
                    addCmd.ExecuteNonQuery();
                    conn.Close();
                    return Json(JsonResponseFactory.ErrorResponse("Rejestracja zakończona sukcesem."),
                        JsonRequestBehavior.DenyGet);
                }
            }
            else
            {
                return Json(JsonResponseFactory.ErrorResponse(""), JsonRequestBehavior.DenyGet);
            }
        }
    }
}
