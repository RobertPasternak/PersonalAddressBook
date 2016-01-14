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
        [Display(Name = "Ulica")]
        public string Street { get; set; }

        [Required]
        [Display(Name = "Miasto")]
        public string City { get; set; }

        [Required]
        [Display(Name = "Kod Pocztowy")]
        public string PostalCode { get; set; }

        [Required]
        [Display(Name = "Kraj")]
        public string Country { get; set; }


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
                    Street = (string) reader["Street"],
                    City = (string)reader["City"],
                    PostalCode = (string)reader["PostalCode"],
                    Country = (string)reader["Country"]
                };

                conn.Close();
                return contactModel;
            }
            return null;
        }


      


    }


}