using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models.DTO
{
    public class ReviewDto
    {
        public Guid Id { get; set; }
        public Guid ProductId { get; set; }
        public string ProductName { get; set; }
        public string ReviewerUsername { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
        public bool IsApproved { get; set; }
    }
}