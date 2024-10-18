using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.Domain;

namespace WebShop.Models.DatabaseModels
{
    public class FavouriteProduct
    {
        public FavouriteProduct()
        {
                
        }
        public FavouriteProduct(Guid id, string name, double price, int quantity, string description, string imageURL, DateTime postDate, string city, bool isAvaliable, bool isDeleted, string username)
        {
            Product = new Product(id, name, price, quantity, description, imageURL, postDate, city, isAvaliable, isDeleted);
            Username = username;
        }
        public Product Product { get; set; }
        public string Username { get; set; }

        public override string ToString()
        {
            return Product.ToString() + $"|{Username}";
        }
    }
}