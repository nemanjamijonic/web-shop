using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Enums;

namespace WebShop.Models.DTO
{
    public class SearchUsersInput
    {
        public string Name { get; set; }
        public string Surname { get; set; }
        public DateTime DateFrom { get; set; }
        public DateTime DateTo { get; set; }
        public SearchRoleType Role { get; set; }
        public bool IsCombined { get; set; }

    }
}