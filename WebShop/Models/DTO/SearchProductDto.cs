using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models.DTO
{
    public class SearchProductDto
    {
        public string Name { get; set; }
        public string City { get; set; }
        public double MinPrice { get; set; }
        public double MaxPrice { get; set; }
        public bool IsCombined { get; set; }
    }
}