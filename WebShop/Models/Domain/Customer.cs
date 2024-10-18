using System;
using System.Collections.Generic;
using WebShop.Models.DTO;
using WebShop.Models.Enums;

namespace WebShop.Models.Domain
{
    public class Customer : User
    {

        public Customer()
            : base()
        {

        }

        public Customer(string username, string password, string name, string surname, Gender gender, string email, DateTime birthDate, bool isDeleted)
            : base(username, password, name, surname, gender, email, birthDate, isDeleted)
        {
            Role = Role.Customer;
        }

        public List<Order> Orders { get; set; } = new List<Order>();
        public List<Product> FavouriteProducts { get; set; } = new List<Product>();

        public static Customer CreateNewInstance(RegisterUser registerInupt) => new Customer()
        {
            Username = registerInupt.Username,
            BirthDate = registerInupt.DateOfBirth,
            Email = registerInupt.Email,
            Gender = registerInupt.Gender,
            Name = registerInupt.Name,
            Password = registerInupt.Password,
            Role = Role.Customer,
            Surname = registerInupt.Surname,
            Orders = new List<Order>(),
            FavouriteProducts = new List<Product>()
        };

       

        public override string ToString()
        {
            return base.ToString();
        }

    }
}