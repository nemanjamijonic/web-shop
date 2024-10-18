using System;
using WebShop.Models.Enums;

namespace WebShop.Models.DTO
{
    public class SortProductDto
    {
       public SortBy SortBy { get; set; }
       public SortOrder SortOrder { get; set; }


    }
}