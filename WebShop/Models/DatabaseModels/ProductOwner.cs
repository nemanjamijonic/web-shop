using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Domain;

namespace WebShop.Models.DatabaseModels
{
    public class ProductOwner
    {

        public ProductOwner()
        {

        }

        public ProductOwner(Guid id, string name, double price, int quantity, string description, string imageURL, DateTime postDate, string city, bool isAvaliable, bool isDeleted, string owner)
        {
            Product = new Product(id, name, price, quantity, description, imageURL, postDate, city, isAvaliable, isDeleted);
            Owner = owner;
        }

        public Product Product { get; set; }
        public string Owner { get; set; }

        public override string ToString()
        {
            return Product.ToString() + $"|{Owner}";
        }


    }
}