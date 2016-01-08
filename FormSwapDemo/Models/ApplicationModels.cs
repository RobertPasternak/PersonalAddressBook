using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web;

namespace FormSwapDemo.Models
{
    public class ContactModel
    {
        public int Id { get; set; }

        [Required]
        [Display(Name = "Imię")]
        public string Name { get; set; }

        [Required]
        [Display(Name = "Nazwisko")]
        public string Surname { get; set; }

        [Required]
        [Display(Name = "Telefon")]
        public string Phone { get; set; }

        [Required]
        [Display(Name = "Adres")]
        public string Address { get; set; }

        public static List<int> Contacts = new List<int>();

        public List<int> GetContactsList()
        {           
            return Contacts;
        } 


        public static ContactModel GetContact(int contactId)
        {
            var conn
                =
                new SqlConnection(
                    "Data Source = (LocalDB)\\MSSQLLocalDB; AttachDbFilename = |DataDirectory|PersonalAdressBookDatabase.mdf; MultipleActiveResultSets = True; Integrated Security = True; Connect Timeout = 30");

            var cmd = new SqlCommand("SELECT * FROM [Contacts] WHERE [Id] = @i")
            {
                Connection = conn
            };
            conn.Open();
            cmd.Parameters.Add(new SqlParameter("@i", SqlDbType.NVarChar)).Value = contactId;
            var reader = cmd.ExecuteReader();


            if (reader.Read())
            {
                var contactModel = new ContactModel
                {
                    Id = (int) reader["Id"],
                    Name = (string) reader["Name"],
                    Surname = (string) reader["Surname"],
                    Phone = (string) reader["Phone"],
                    Address = (string) reader["Address"]
                };

                conn.Close();
                return contactModel;
            }
            return null;
        }


      


    }


}