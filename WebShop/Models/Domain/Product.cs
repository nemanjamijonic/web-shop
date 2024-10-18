using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.DTO;

namespace WebShop.Models.Domain
{
    public class Product
    {
        public Product()
        {

        }
        public Product(Guid id, string name, double price, int quantity, string description, string imageURL, DateTime postDate, string city, bool isAvaliable, bool isDeleted)
        {
            Id = id;
            Name = name;
            Price = price;
            Quantity = quantity;
            Description = description;
            ImageURL = imageURL;
            PostDate = postDate;
            City = city;
            IsAvaliable = isAvaliable;
            IsDeleted = isDeleted;
        }

        public Guid Id { get; set; }
        public string Name { get; set; }
        public double Price { get; set; }
        public int Quantity { get; set; }
        public string Description { get; set; }
        public string ImageURL { get; set; }
        public DateTime PostDate { get; set; }
        public string City { get; set; }
        public bool IsDeleted { get; set; }
        public List<Review> Reviews { get; set; } = new List<Review>();

        private bool isAvailable;

        public bool IsAvaliable
        {
            get { return Quantity > 0; }
            set { isAvailable = value; }
        }

        public static Product CreateNewInstance(CreateProductInput createProductInput) => new Product()
        {
            Id = Guid.NewGuid(),
            Description = createProductInput.Description,
            City = createProductInput.City,
            ImageURL = createProductInput.ImageURL,
            Name = createProductInput.Name,
            Price = createProductInput.Price,
            Quantity = createProductInput.Quantity,
            Reviews = new List<Review>(),
            IsAvaliable = true,
            PostDate = DateTime.Now,
            IsDeleted = false
        };


        public void UpdateProduct(EditProductInput editProductInput)
        {
            Description = editProductInput.Description;
            City = editProductInput.City;
            ImageURL = editProductInput.ImageURL;
            Name = editProductInput.Name;
            Price = editProductInput.Price;
            Quantity = editProductInput.Quantity;
            IsAvaliable = Quantity > 0;
            IsDeleted = false;
        }



        public override string ToString()
        {
            return $"{Id}|{Name}|{Price}|{Quantity}|{Description}|{ImageURL}|{PostDate.ToString("dd-MM-yyyy")}|{City}|{IsAvaliable}|{IsDeleted}";
        }
    }
}