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
    public class ApplicationController : Controller
    {
        public ActionResult Menu(string condition)
        {
            ViewData["SearchCondition"] = condition;
            
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var user = Session["user"] as LoginModel;

                var conn
                    =
                    new SqlConnection(
                        "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

                var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l")
                {
                    Connection = conn
                };
                conn.Open();
                cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = @user.Login;
                var reader = cmd.ExecuteReader();

                int userId = 0;

                if (reader.Read())
                {
                    userId = (int) reader["Id"];
                }
                List<ContactModel> contacts = new List<ContactModel>();


                if (string.IsNullOrEmpty(condition))
                {
                    cmd = new SqlCommand("SELECT * FROM [Contacts] WHERE [UserId] = @u") {Connection = conn};
                    cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int)).Value = userId;
                    reader = cmd.ExecuteReader();



                    while (reader.Read())
                    {
                        var contactData = new ContactModel
                        {
                            Id = (int) reader["Id"],
                            Name = (string) reader["Name"],
                            Surname = (string) reader["Surname"],
                            Phone = (string) reader["Phone"],
                            Address = (string) reader["Address"]
                        };

                        contacts.Add(contactData);
                    }

                }
                else
                {

                    cmd = new SqlCommand("SELECT * FROM [Contacts] WHERE  [UserId] = @u  AND ( [Name] LIKE @n OR [Surname] LIKE @n OR [Phone] LIKE @n OR [Address] LIKE @n )") { Connection = conn };                   
                    cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int)).Value = userId;
                    cmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar)).Value = '%' + condition + '%';
                    reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var contactData = new ContactModel
                        {
                            Id = (int)reader["Id"],
                            Name = (string)reader["Name"],
                            Surname = (string)reader["Surname"],
                            Phone = (string)reader["Phone"],
                            Address = (string)reader["Address"]
                        };

                        contacts.Add(contactData);
                    }
                }

                conn.Close();
                return View(contacts);
            }
        }


        public ActionResult Logout()
        {
            Session.Clear();
            return RedirectToAction("Index", "Home");
        }

        [HttpGet]
        public ActionResult UserProfile()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public ActionResult UserProfile(ProfileModel profileModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var user = Session["user"] as LoginModel;


                    var conn
                        =
                        new SqlConnection(
                            "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

                    var cmd = new SqlCommand("SELECT [Login] FROM [Users] WHERE [Login] = @l AND [Password] = @p")
                    {
                        Connection = conn
                    };
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = @user.Login;
                    cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = profileModel.OldPassword;
                    var reader = cmd.ExecuteReader();

                    if (reader.HasRows)
                    {
                        var updCmd =
                            new SqlCommand(
                                "UPDATE [Users] SET [LoginAttempts] = @a , [Password] = @p WHERE [Login] = @l")
                            {
                                Connection = conn
                            };

                        updCmd.Parameters.Clear();
                        updCmd.Parameters.Add(new SqlParameter("@a", SqlDbType.Int)).Value = 0;
                        updCmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value =
                            profileModel.NewPassword;
                        updCmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = @user.Login;
                        updCmd.ExecuteNonQuery();
                        conn.Close();
                        return RedirectToAction("Menu", "Application");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Błędne hasło użytkownika.");
                        conn.Close();
                        return View();
                    }
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult AddContact()
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View();
        }


        [HttpPost]
        public ActionResult AddContact(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var user = Session["user"] as LoginModel;


                    var conn
                        =
                        new SqlConnection(
                            "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

                    var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l")
                    {
                        Connection = conn
                    };
                    conn.Open();
                    cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = @user.Login;
                    var reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        int userId = (int) reader["Id"];

                        var addCmd =
                            new SqlCommand(
                                "INSERT INTO [Contacts] ([Name], [Surname], [Phone], [Address], [UserId]) VALUES (@n, @s, @p, @a, @u)")
                            {
                                Connection = conn
                            };
                        addCmd.Parameters.Clear();
                        addCmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar)).Value = contactModel.Name;
                        addCmd.Parameters.Add(new SqlParameter("@s", SqlDbType.NVarChar)).Value = contactModel.Surname;
                        addCmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value =
                            contactModel.Phone;
                        addCmd.Parameters.Add(new SqlParameter("@a", SqlDbType.NVarChar)).Value =
                            contactModel.Address;
                        addCmd.Parameters.Add(new SqlParameter("@u", SqlDbType.Int)).Value = userId;

                        addCmd.ExecuteNonQuery();
                        conn.Close();
                        return RedirectToAction("Menu", "Application");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Wystąpił nieznany błąd.");
                        conn.Close();
                        return View();
                    }
                }
            }

            else
            {
                return View();
            }
        }


        [HttpGet]
        public ActionResult EditContact(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(ContactModel.GetContact(id));
        }

        [HttpPost]
        public ActionResult EditContact(ContactModel contactModel)
        {
            if (ModelState.IsValid)
            {
                if (Session["user"] == null)
                {
                    return RedirectToAction("Index", "Home");
                }
                else
                {
                    var conn
                        =
                        new SqlConnection(
                            "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");


                    var updCmd =
                        new SqlCommand(
                            "UPDATE [Contacts] SET [Name] = @n , [Surname] = @s , [Phone] = @p , [Address] = @a WHERE [Id] = @i")
                        {
                            Connection = conn
                        };
                    conn.Open();
                    updCmd.Parameters.Clear();
                    updCmd.Parameters.Add(new SqlParameter("@n", SqlDbType.NVarChar)).Value = contactModel.Name;
                    updCmd.Parameters.Add(new SqlParameter("@s", SqlDbType.NVarChar)).Value = contactModel.Surname;
                    updCmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = contactModel.Phone;
                    updCmd.Parameters.Add(new SqlParameter("@a", SqlDbType.NVarChar)).Value = contactModel.Address;
                    updCmd.Parameters.Add(new SqlParameter("@i", SqlDbType.Int)).Value = contactModel.Id;
                    updCmd.ExecuteNonQuery();
                    conn.Close();
                    return RedirectToAction("Menu", "Application");
                }
            }
            else
            {
                return View();
            }
        }

        [HttpGet]
        public ActionResult DeleteContact(int id)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            return View(ContactModel.GetContact(id));
        }

        [HttpPost]
        public ActionResult DeleteContact(ContactModel contactModel)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }

            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            var delCmd =
                new SqlCommand(
                    "DELETE FROM [Contacts] WHERE [Id] = @i")
                {
                    Connection = conn
                };
            conn.Open();
            delCmd.Parameters.Clear();
            delCmd.Parameters.Add(new SqlParameter("@i", SqlDbType.NVarChar)).Value = contactModel.Id;
            delCmd.ExecuteNonQuery();
            conn.Close();
            return RedirectToAction("Menu", "Application");
        }


        public ActionResult ShowContactGroup(string[] ids)
        {
            if (Session["user"] == null)
            {
                return RedirectToAction("Index", "Home");
            }
            else
            {
                var conn
                    =
                    new SqlConnection(
                        "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");
                conn.Open();

                List<ContactModel> contacts = new List<ContactModel>();
                int[] id = null;
                if (ids != null)
                {
                    id = new int[ids.Length];
                    int j = 0;
                    foreach (string i in ids)
                    {
                        int.TryParse(i, out id[j++]);
                    }
                }


                if (id != null)
                {
                    for (int k = 0; k < id.Length; k++)
                    {
                        var cmd = new SqlCommand("SELECT * FROM [Contacts] WHERE [Id] = @i") {Connection = conn};
                        cmd.Parameters.Clear();
                        cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.Int)).Value = id[k];
                        var reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var contactData = new ContactModel
                            {
                                Id = (int) reader["Id"],
                                Name = (string) reader["Name"],
                                Surname = (string) reader["Surname"],
                                Phone = (string) reader["Phone"],
                                Address = (string) reader["Address"]
                            };

                            contacts.Add(contactData);
                        }
                    }
                }
                conn.Close();
                return View(contacts);
            }
        }


        public ActionResult DeleteContactGroup(ContactModel contactModel)
        {
            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            conn.Open();
            foreach (var item in contactModel.GetContactsList())
            {
                var delCmd = new SqlCommand("DELETE FROM [Contacts] WHERE [Id] = @i") {Connection = conn};
                delCmd.Parameters.Clear();
                delCmd.Parameters.Add(new SqlParameter("@i", SqlDbType.Int)).Value = item;
                delCmd.ExecuteNonQuery();
            }
            conn.Close();
            return RedirectToAction("Menu", "Application");
        }
    }
}
