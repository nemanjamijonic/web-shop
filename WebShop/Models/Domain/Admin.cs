using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Enums;

namespace WebShop.Models.Domain
{
    public class Admin : User
    {

        public Admin(string username, string password, string name, string surname, Gender gender, string email, DateTime birthDate, bool isDeleted)
            : base(username, password, name, surname, gender, email, birthDate, isDeleted)
        {
            Role = Role.Admin;
        }

        public override string ToString()
        {
            return base.ToString();
        }
    }
}