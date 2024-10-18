using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Enums;

namespace WebShop.Models.DTO
{
    public class SortUsersInput
    {
        public SortUserBy SortBy { get; set; }
        public SortOrder SortOrder { get; set; }

    }
}