using System;
using System.Collections.Generic;
using WebShop.Models.DTO;
using WebShop.Models.Enums;

namespace WebShop.Models.Domain
{
    public class Salesman : User
    {
        public Salesman()
        {

        }
        public Salesman(string username, string password, string name, string surname, Gender gender, string email, DateTime birthDate, bool isDeleted)
            : base(username, password, name, surname, gender, email, birthDate, isDeleted)
        {
            Role = Role.Salesman;
        }

        public List<Product> PublishedProducts { get; set; } = new List<Product>();

        public static Salesman CreateNewInstance(CreateSalesmanInput registerInupt) => new Salesman()
        {
            Username = registerInupt.Username,
            BirthDate = registerInupt.DateOfBirth,
            Email = registerInupt.Email,
            Gender = registerInupt.Gender,
            Name = registerInupt.Name,
            Password = registerInupt.Password,
            Role = Role.Salesman,
            Surname = registerInupt.Surname,
            PublishedProducts = new List<Product>()
        };

        public override string ToString()
        {
            return base.ToString();
        }
    }
}