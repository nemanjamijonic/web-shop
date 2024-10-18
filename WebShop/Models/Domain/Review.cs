using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebShop.Models.DTO;

namespace WebShop.Models.Domain
{
    public class Review
    {
        public Guid Id { get; set; }
        public Product Product { get; set; }
        public Customer Reviewer { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public bool IsDeleted { get; set; }
        public bool IsApproved { get; set; }

        public void Update(UpdateReviewInput updateReviewInput)
        {
            Title = updateReviewInput.Title;
            Content = updateReviewInput.Content;
            ImageURL = updateReviewInput.ImageURL;
        }


        public override string ToString()
        {
            return $"{Id}|" + Product.ToString() + "|" + Reviewer.ToString() + $"|{Title}|{Content}|{ImageURL}|{IsDeleted}|{IsApproved}"; 
        }


    }
}