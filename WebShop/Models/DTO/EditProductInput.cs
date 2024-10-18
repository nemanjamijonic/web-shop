using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models.DTO
{
    public class EditProductInput
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public string City { get; set; }
    }
}