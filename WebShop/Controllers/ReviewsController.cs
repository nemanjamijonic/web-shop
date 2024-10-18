using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShop.Models.Domain;
using WebShop.Models.DTO;
using WebShop.Models.Enums;

namespace WebShop.Controllers
{
    [RoutePrefix("reviews")]
    public class ReviewsController : ApiController
    {
        [HttpGet]
        [Route("GetProductReviews")]
        public IHttpActionResult GetProductReviews([FromUri] Guid productId)
        {

            List<Review> allReviews = Database.Database.GetAllReviews();
            allReviews = allReviews.Where(x => !x.IsDeleted && x.Product.Id == productId && x.IsApproved).ToList();


            List<ReviewDto> results = new List<ReviewDto>();

            foreach(var review in allReviews)
            {
                results.Add(new ReviewDto()
                {
                    Id = review.Id,
                    Content = review.Content,
                    ImageURL = review.ImageURL,
                    ProductId = review.Product.Id,
                    ProductName = review.Product.Name,
                    ReviewerUsername = review.Reviewer.Username,
                    Title = review.Title,
                    IsApproved = review.IsApproved
                });
            }

            
            return Ok(results);

        }

        [HttpGet]
        [Route("GetReview")]
        public IHttpActionResult GetReview([FromUri] Guid reviewId)
        {

            List<Review> allReviews = Database.Database.GetAllReviews();
            var review = allReviews.FirstOrDefault(x => !x.IsDeleted && x.Id == reviewId && x.IsApproved);


            ReviewDto result = new ReviewDto()
            {
                Id = review.Id,
                Content = review.Content,
                ImageURL = review.ImageURL,
                ProductId = review.Product.Id,
                ProductName = review.Product.Name,
                ReviewerUsername = review.Reviewer.Username,
                Title = review.Title,
                IsApproved = review.IsApproved
            };



            return Ok(result);

        }
    }
}
