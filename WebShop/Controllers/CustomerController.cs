using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Web;
using System.Web.Http;
using WebShop.Models.DatabaseModels;
using WebShop.Models.Domain;
using WebShop.Models.DTO;
using WebShop.Models.Enums;

namespace WebShop.Controllers
{
    [RoutePrefix("customers")]
    public class CustomerController : ApiController
    {

        [HttpGet]
        [Route("GetOrders")]
        public IHttpActionResult GetOrders()
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role == Role.Salesman)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            List<Order> orders = (List<Order>)HttpContext.Current.Application["orders"];
            var results =  orders.Where(x => x.Customer.Username == loggedIn.Username).OrderByDescending(x => x.OrderDate).ToList(); ;

            return Ok(results);

        }

        [HttpGet]
        [Route("GetFavouriteProducts")]
        public IHttpActionResult GetFavouriteProducts()
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            Customer customer = (Customer)loggedIn;

            var res = customer.FavouriteProducts.Where(x => !x.IsDeleted).ToList();

            return Ok(res);


        }

        [HttpPost]
        [Route("MarkAsFavourite")]
        public IHttpActionResult MarkAsFavourite([FromBody] FavouriteProductInput favouriteProductInput)
        {

            if (favouriteProductInput.ProductId == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if(loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if(loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            Product foundProduct = products.FirstOrDefault(x => x.Id == favouriteProductInput.ProductId);

            if(foundProduct == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find product with id {favouriteProductInput.ProductId}");
            }

            if(foundProduct.IsDeleted)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {favouriteProductInput.ProductId} is deleted!");
            }


            List<Product> favouriteProducts = Database.Database.GetUserFavouriteProducts(loggedIn.Username);
            if(favouriteProducts.Any(x => x.Id == favouriteProductInput.ProductId))
                return Content(HttpStatusCode.BadRequest, $"Product with id = {favouriteProductInput.ProductId} is already added to favourites!");

            Customer customer = (Customer)loggedIn;
            customer.FavouriteProducts.Add(foundProduct);

            FavouriteProduct favouriteProduct = new FavouriteProduct()
            {
                Product = foundProduct,
                Username = customer.Username
            };

            Database.Database.InsertEntity(favouriteProduct, Database.Database.favouriteProductsPath);
            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();

            return Ok(foundProduct);



        }


        [HttpPost]
        [Route("CreateOrder")]
        public IHttpActionResult CreateOrder([FromBody] OrderProductInput orderProductInput)
        {

            if (orderProductInput.ProductId == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            Product foundProduct = products.FirstOrDefault(x => x.Id == orderProductInput.ProductId && x.IsAvaliable);

            if (foundProduct == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find product with id {orderProductInput.ProductId}");
            }

            if (foundProduct.IsDeleted)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {orderProductInput.ProductId} is deleted!");
            }

            if (foundProduct.Quantity < orderProductInput.Quantity)
            {
                return Content(HttpStatusCode.BadRequest, $"Only {foundProduct.Quantity} products left!");
            }


            Customer customer = (Customer)loggedIn;

            Order order = new Order(foundProduct, orderProductInput.Quantity, customer);
            foundProduct.Quantity -= orderProductInput.Quantity;

            Database.Database.InsertEntity(order, Database.Database.ordersPath);
            Database.Database.InsertEntities(products, Database.Database.productsPath);

            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();
            HttpContext.Current.Application["orders"] = Database.Database.GetAllOrders();


            return Ok(order);



        }

        [HttpPost]
        [Route("CompleteOrder")]
        public IHttpActionResult CompleteOrder([FromBody] CompleteOrderInput completeOrderInput)
        {

            if (completeOrderInput.OrderId == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role == Role.Salesman)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Order> orders = Database.Database.GetAllOrders();

            Order foundOrder = orders.FirstOrDefault(x => x.Id == completeOrderInput.OrderId);

            if (foundOrder == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find order with id {completeOrderInput.OrderId}");
            }

            if (foundOrder.OrderState != OrderState.Active)
            {
                return Content(HttpStatusCode.BadRequest, $"Order is not active!");
            }

            foundOrder.OrderState = OrderState.Completed;

            Database.Database.InsertEntities(orders, Database.Database.ordersPath);

            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["orders"] = Database.Database.GetAllOrders();


            return Ok(foundOrder);



        }



        [HttpPost]
        [Route("CreateReview")]
        public IHttpActionResult CreateReview([FromBody] CreateReviewInput createReviewInput)
        {

            if (createReviewInput.ProductId == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if(string.IsNullOrEmpty(createReviewInput.Title))
            {
                return Content(HttpStatusCode.BadRequest, "Empty title!");
            }

            if (string.IsNullOrEmpty(createReviewInput.Content))
            {
                return Content(HttpStatusCode.BadRequest, "Empty Content!");
            }

            if (string.IsNullOrEmpty(createReviewInput.ImageURL))
            {
                return Content(HttpStatusCode.BadRequest, "Empty ImageURL!");
            }

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            Product foundProduct = products.FirstOrDefault(x => x.Id == createReviewInput.ProductId && !x.IsDeleted);

            if (foundProduct == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find product with id {createReviewInput.ProductId}");
            }


            List<Order> orders = (List<Order>)HttpContext.Current.Application["orders"];

            Order foundOrder = orders.FirstOrDefault(x => x.Customer.Username == loggedIn.Username && x.Product.Id == createReviewInput.ProductId && x.OrderState == OrderState.Completed);

            if (foundOrder == null)
            {
                return Content(HttpStatusCode.NotFound, $"User doesn't have completed order for product with id = {createReviewInput.ProductId}");
            }


            Review review = new Review()
            {
                Id = Guid.NewGuid(),
                Reviewer = (Customer)loggedIn,
                Product = foundProduct,
                Content = createReviewInput.Content,
                Title = createReviewInput.Title,
                ImageURL = createReviewInput.ImageURL,
               
            };

            foundProduct.Reviews.Add(review);


            Database.Database.InsertEntity(review, Database.Database.reviewsPath);

            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();


            return Ok(createReviewInput);



        }


        [HttpPut]
        [Route("UpdateReview")]
        public IHttpActionResult UpdateReview([FromBody] UpdateReviewInput updateReviewInput)
        {

            if (updateReviewInput.Id == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if (string.IsNullOrEmpty(updateReviewInput.Title))
            {
                return Content(HttpStatusCode.BadRequest, "Empty title!");
            }

            if (string.IsNullOrEmpty(updateReviewInput.Content))
            {
                return Content(HttpStatusCode.BadRequest, "Empty Content!");
            }

            if (string.IsNullOrEmpty(updateReviewInput.ImageURL))
            {
                return Content(HttpStatusCode.BadRequest, "Empty ImageURL!");
            }

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            Customer customer = (Customer)loggedIn;

            List<Review> allReviews = Database.Database.GetAllReviews();

            Review foundReview = allReviews.FirstOrDefault(x => x.Id == updateReviewInput.Id && x.Reviewer.Username == customer.Username && !x.IsDeleted);

            if (foundReview == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find review with id {updateReviewInput.Id}");
            }

            foundReview.Update(updateReviewInput);

            Database.Database.InsertEntities(allReviews, Database.Database.reviewsPath);
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();


            return Ok(updateReviewInput);



        }

        [HttpDelete]
        [Route("DeleteReview")]
        public IHttpActionResult DeleteReview([FromBody] DeleteReviewUpdate deleteReviewUpdate)
        {

            if (deleteReviewUpdate.Id == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            Customer customer = (Customer)loggedIn;

            List<Review> allReviews = Database.Database.GetAllReviews();

            Review foundReview = allReviews.FirstOrDefault(x => x.Id == deleteReviewUpdate.Id && x.Reviewer.Username == customer.Username);

            if (foundReview == null)
            {
                return Content(HttpStatusCode.NotFound, $"Couldn't find review with id {deleteReviewUpdate.Id}");
            }

            foundReview.IsDeleted = true;

            Database.Database.InsertEntities(allReviews, Database.Database.reviewsPath);
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();


            return Ok(deleteReviewUpdate);



        }
    }
}
