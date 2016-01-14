using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data;
using System.Data.Entity;
using System.Data.OleDb;
using System.Data.SqlClient;
using System.Globalization;
using System.Web.Security;

namespace FormSwapDemo.Models
{
    public class LoginModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [Display(Name = "Hasło")]
        public string Password { get; set; }


        public bool IsValidUser(string login, string password)
        {
            var conn =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");
            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l ") {Connection = conn};
            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = login;
            conn.Open();
            var reader = cmd.ExecuteReader();


            if (reader.HasRows)
            {
                cmd = new SqlCommand("SELECT [Login] FROM [Users] WHERE [Login] = @l AND [Password] = @p")
                {
                    Connection = conn
                };
                cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = login;
                cmd.Parameters.Add(new SqlParameter("@p", SqlDbType.NVarChar)).Value = password;
                reader = cmd.ExecuteReader();

                if (reader.HasRows)
                {
                    conn.Close();
                    return true;
                }
                else
                {
                    //zwiększ liczbę nieudanych podejść do logowania
                    var updCmd =
                        new SqlCommand("UPDATE [Users] SET [LoginAttempts] = [LoginAttempts] + 1 WHERE [Login] = @l")
                        {
                            Connection = conn
                        };

                    updCmd.Parameters.Clear();
                    updCmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = login;
                    updCmd.ExecuteNonQuery();
                    conn.Close();
                    return false;
                }
            }
            else
            {
                conn.Close();
                return false;
            }
        }
    }

    public class RegisterModel
    {
        [Required]
        [Display(Name = "Login")]
        public string Login { get; set; }

        [Required]
        [StringLength(100, ErrorMessage = "{0} musi mieć co najmniej {2} znaków długości.", MinimumLength = 6)]
        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Required]
        [Display(Name = "Potwierdź Hasło")]
        [Compare("Password", ErrorMessage = "Hasła nie są identyczne.")]
        public string ConfirmPassword { get; set; }

        [Required]
        [Display(Name = "Sekretne Pytanie")]
        public string SecretQuestion { get; set; }

        [Required]
        [Display(Name = "Sekretna Odpowiedź")]
        public string SecretAnswer { get; set; }
    }

    public class RemindModel
    {
        public int Id { get; set; }

        [Display(Name = "Login")]
        public string Login { get; set; }

        [Display(Name = "Hasło")]
        public string Password { get; set; }

        [Display(Name = "Sekretne Pytanie")]
        public string SecretQuestion { get; set; }

        [Display(Name = "Poprawna Sekretna Odpowiedź")]
        public string SecretAnswer { get; set; }

        [Display(Name = "Sekretna Odpowiedź")]
        public string UserSecretAnswer { get; set; }


        public static RemindModel GetUserData(string userLogin)
        {
            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @u")
            {
                Connection = conn
            };
            conn.Open();
            cmd.Parameters.Add(new SqlParameter("@u", SqlDbType.NVarChar)).Value = userLogin;
            var reader = cmd.ExecuteReader();


            if (reader.Read())
            {
                var remindModel = new RemindModel
                {
                    SecretQuestion = (string) reader["SecretQuestion"]
                };

                conn.Close();
                return remindModel;
            }
            return null;
        }


        public static bool IsUserAnswerCorrect(string userLogin, string userAnswer)
        {
            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @login AND [SecretAnswer] = @answer")
            {
                Connection = conn
            };
            conn.Open();
            cmd.Parameters.Add(new SqlParameter("@login", SqlDbType.NVarChar)).Value = userLogin;
            cmd.Parameters.Add(new SqlParameter("@answer", SqlDbType.NVarChar)).Value = userAnswer;
            var reader = cmd.ExecuteReader();
            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }
    }


    public class ProfileModel
    {
        public string Login { get; set; }

        [Required]
        [Display(Name = "Stare Hasło")]
        public string OldPassword { get; set; }

        [Required]
        [Display(Name = "Nowe Hasło")]
        public string NewPassword { get; set; }

        [Required]
        [Display(Name = "Potwierdź Nowe Hasło")]
        [Compare("NewPassword", ErrorMessage = "Nowe hasła nie są identyczne.")]
        public string ConfirmNewPassword { get; set; }

        public int LoginAttempts { get; set; }


        public static bool IsUnsafeUser(string login)
        {
            var conn =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");
            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l AND [LoginAttempts] > 4")
            {
                Connection = conn
            };
            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = login;
            conn.Open();
            var reader = cmd.ExecuteReader();

            if (reader.HasRows)
            {
                conn.Close();
                return true;
            }
            else
            {
                conn.Close();
                return false;
            }
        }

        public static int GetNumberOfAttempts(string login)
        {
            var conn =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");
            var cmd = new SqlCommand("SELECT * FROM [Users] WHERE [Login] = @l") {Connection = conn};
            cmd.Parameters.Add(new SqlParameter("@l", SqlDbType.NVarChar)).Value = login;
            conn.Open();
            var reader = cmd.ExecuteReader();
            if (reader.Read())
            {
                int attempts = (int) reader["LoginAttempts"];
                conn.Close();
                return attempts;
            }
            conn.Close();
            return 0;
        }
    }
}
