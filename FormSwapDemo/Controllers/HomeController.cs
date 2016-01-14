using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using FormSwapDemo.Models;

namespace FormSwapDemo.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            Session.Clear();
            return View();
        }


        [HttpGet]
        public ActionResult RemindPasswordStep1()
        {
            return View();
        }


        [HttpPost]
        public ActionResult RemindPasswordStep1(RemindModel remindModel)
        {
            string userLogin = "";

            if (!string.IsNullOrEmpty(remindModel.Login))
            {
                userLogin = remindModel.Login;
            }

            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l")
            {
                Connection = conn
            };
            conn.Open();
            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = userLogin;
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                Session["reminder"] = new RemindModel() {Login = remindModel.Login};
                conn.Close();
                return RedirectToAction("RemindPasswordStep2", "Home");
            }
            else
            {
                conn.Close();
                ModelState.AddModelError("", "Nieprawidłowa nazwa użytkownika.");
                return View();
            }
        }


        [HttpGet]
        public ActionResult RemindPasswordStep2()
        {
            if (Session["reminder"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var reminder = Session["reminder"] as RemindModel;

                return View(RemindModel.GetUserData(@reminder.Login));
            }
        }


        [HttpPost]
        public ActionResult RemindPasswordStep2(RemindModel remindModel)
        {
            if (Session["reminder"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var reminder = Session["reminder"] as RemindModel;


            if (RemindModel.IsUserAnswerCorrect(@reminder.Login, remindModel.UserSecretAnswer))
            {
                return RedirectToAction("RemindPasswordStep3", "Home");
            }
            else
            {
                ModelState.AddModelError("", "Nieprawidłowa odpowiedź!");
                return View(remindModel);
            }
        }


        public ActionResult RemindPasswordStep3(RemindModel remindModel)
        {
            if (Session["reminder"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            else
            {
                string newGeneratedPassword = System.Web.Security.Membership.GeneratePassword(8, 0);

                remindModel.Password = newGeneratedPassword;

                var reminder = Session["reminder"] as RemindModel;

                var conn
                    =
                    new SqlConnection(
                        "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");


                var updCmd =
                    new SqlCommand(
                        "UPDATE [Users] SET [Password] = @password WHERE [Login] = @login")
                    {
                        Connection = conn
                    };
                conn.Open();
                updCmd.Parameters.Clear();
                updCmd.Parameters.Add(new SqlParameter("@password", SqlDbType.NVarChar)).Value = newGeneratedPassword;
                updCmd.Parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar)).Value = @reminder.Login;
                updCmd.ExecuteNonQuery();
                conn.Close();
            }

            return View(remindModel);
        }
    }
}
