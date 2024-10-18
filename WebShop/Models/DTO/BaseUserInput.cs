using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Enums;

namespace WebShop.Models.DTO
{
    public class BaseUserInput
    {
        public string Username { get; set; }
        public string Password { get; set; }
        public string ConfirmPassword { get; set; }
        public string Name { get; set; }
        public string Surname { get; set; }
        public Gender Gender { get; set; }
        public string Email { get; set; }
        public DateTime DateOfBirth { get; set; }
    }
}