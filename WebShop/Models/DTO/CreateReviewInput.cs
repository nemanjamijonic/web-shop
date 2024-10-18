using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WebShop.Models.DTO
{
    public class CreateReviewInput
    {
        public Guid ProductId { get; set; }
        public string Title { get; set; }
        public string Content { get; set; }
        public string ImageURL { get; set; }
    }
}