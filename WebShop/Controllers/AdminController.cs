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
    [RoutePrefix("admins")]
    public class AdminController : ApiController
    {
        [HttpGet]
        [Route("GetAllUsers")]
        public IHttpActionResult GetAllUsers()
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            var users  = (List<User>)HttpContext.Current.Application["users"];
            var res = users.Where(x => !x.IsDeleted && (x.Role == Role.Salesman || x.Role == Role.Customer)).ToList(); 

            return Ok(res);

        }

        [HttpGet]
        [Route("GetByUsername")]
        public IHttpActionResult GetByUsername([FromUri] string username)
        {
            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            var users = (List<User>)HttpContext.Current.Application["users"];
            var res = users.FirstOrDefault(x => x.Username == username);

            return Ok(res);
        }

        [HttpGet]
        [Route("GetAllOrders")]
        public IHttpActionResult GetAllOrders()
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            List<Order> orders = (List<Order>)HttpContext.Current.Application["orders"];
            var results = orders.OrderByDescending(x => x.OrderDate).ToList(); ;

            return Ok(results);


        }

        [HttpGet]
        [Route("GetProductReviews")]
        public IHttpActionResult GetProductReviews([FromUri] Guid productId)
        {

            List<Review> allReviews = Database.Database.GetAllReviews();
            allReviews = allReviews.Where(x => !x.IsDeleted && x.Product.Id == productId).ToList();


            List<ReviewDto> results = new List<ReviewDto>();

            foreach (var review in allReviews)
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

        [HttpPost]
        [Route("Search")]
        public List<User> Search([FromBody] SearchUsersInput searchInput)
        {
            List<User> users = (List<User>)HttpContext.Current.Application["users"];
            List<User> results = new List<User>();

            users = users.Where(x => x.Role == Role.Salesman || x.Role == Role.Customer).ToList();


            if (searchInput.IsCombined)
            {

                if (!string.IsNullOrEmpty(searchInput.Name))
                {
                    results = users.Where(x => x.Name.ToLower().Contains(searchInput.Name.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(searchInput.Surname))
                {
                    results = results.Where(x => x.Surname.ToLower().Contains(searchInput.Surname.ToLower())).ToList();
                }

                if(searchInput.Role == SearchRoleType.Customer)
                {
                    results = results.Where(x => x.Role == Role.Customer).ToList();
                }


                if (searchInput.Role == SearchRoleType.Salesman)
                {
                    results = results.Where(x => x.Role == Role.Salesman).ToList();
                }

                if (IsDateValid(searchInput.DateFrom, searchInput.DateTo))
                {
                    results = results.Where(x => x.BirthDate >= searchInput.DateFrom && x.BirthDate <= searchInput.DateTo).ToList();
                }

                return results;

            }

            var searchRes = users;
            if (searchInput.Role == SearchRoleType.Customer)
            {
                searchRes = searchRes.Where(x => x.Role == Role.Customer).ToList();
            }



            if (searchInput.Role == SearchRoleType.Salesman)
            {
                searchRes = searchRes.Where(x => x.Role == Role.Salesman).ToList();
            }

            if (!string.IsNullOrEmpty(searchInput.Name) || !string.IsNullOrEmpty(searchInput.Surname) || IsDateValid(searchInput.DateFrom, searchInput.DateTo)) 
            {
                searchRes = users.Where(x => (!string.IsNullOrEmpty(searchInput.Name) && x.Name.ToLower().Contains(searchInput.Name.ToLower()))
                                       || (!string.IsNullOrEmpty(searchInput.Surname) && x.Surname.ToLower().Contains(searchInput.Surname.ToLower()))
                                       || (IsDateValid(searchInput.DateFrom, searchInput.DateTo) && x.BirthDate >= searchInput.DateFrom && x.BirthDate <= searchInput.DateTo)).ToList();
            }
            


            return searchRes;

        }


        [HttpPost]
        [Route("Sort")]
        public List<User> Sort([FromBody] SortUsersInput sortInput)
        {

            List<User> users = (List<User>)HttpContext.Current.Application["users"];

            users = users.Where(x => x.Role == Role.Salesman || x.Role == Role.Customer).ToList();

            return SortProductsByField(users, sortInput.SortBy, sortInput.SortOrder);

        }


        [HttpPost]
        [Route("CreateSalesman")]
        public IHttpActionResult CreateSalesman([FromBody] CreateSalesmanInput createSalesmanInput)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            var validation = ValidateRegisterInputValid(createSalesmanInput);

            if (validation != null)
                return validation;

            List<User> list = (List<User>)HttpContext.Current.Application["users"];

            if (list.Any(x => x.Username == createSalesmanInput.Username))
            {
                return Content(HttpStatusCode.BadRequest, "User with this username already exists!");
            }

            Salesman salesman = Salesman.CreateNewInstance(createSalesmanInput);

            Database.Database.InsertEntity(salesman, Database.Database.salesmanPath);
            HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();
            HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

            return Content(HttpStatusCode.Created, salesman);

        }

        [HttpPost]
        [Route("Filter")]
        public IHttpActionResult Sort([FromBody] ProductFilterInput filter)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];




            if (filter.Filter == ProductFilterType.All)
            {
                return Ok(products);
            }
            else if (filter.Filter == ProductFilterType.Available)
            {
                var res = products.Where(x => x.IsAvaliable);
                res = res.Where(x => !x.IsDeleted).ToList();
                return Ok(res);
            }
            else
            {
                var res = products.Where(x => !x.IsAvaliable);
                res = res.Where(x => !x.IsDeleted).ToList();
                return Ok(res);
            }

        }


        [HttpPut]
        [Route("Edit")]
        public IHttpActionResult Edit([FromBody] UpdateUserProfile editSalesman)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            var validation = ValidateRegisterInputValid(editSalesman);

            if (validation != null)
                return validation;

            List<User> users = (List<User>)HttpContext.Current.Application["users"];

            var foundUser = users.FirstOrDefault(x => x.Username == editSalesman.Username);

            if (foundUser == null)
            {
                return Content(HttpStatusCode.NotFound, $"User {editSalesman.Username} not found!");
            }

            Role role = foundUser.Role;

            if (role == Role.Customer)
            {
                List<Customer> customers = (List<Customer>)HttpContext.Current.Application["customers"];
                Customer customer = customers.First(x => x.Username == editSalesman.Username);
                customer.UpdateProfile(editSalesman);

                Database.Database.InsertEntities(customers, Database.Database.customersPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

                return Content(HttpStatusCode.OK, customer);

            }

            if (role == Role.Admin)
            {
                List<Admin> admins = (List<Admin>)HttpContext.Current.Application["admins"];
                Admin admin = admins.First(x => x.Username == editSalesman.Username);
                admin.UpdateProfile(editSalesman);

                Database.Database.InsertEntities(admins, Database.Database.adminsPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

                return Content(HttpStatusCode.OK, admin);

            }

            if (role == Role.Salesman)
            {
                List<Salesman> salesmans = (List<Salesman>)HttpContext.Current.Application["salesmans"];
                Salesman salesman = salesmans.First(x => x.Username == editSalesman.Username);
                salesman.UpdateProfile(editSalesman);

                Database.Database.InsertEntities(salesmans, Database.Database.salesmanPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

                return Content(HttpStatusCode.OK, salesman);

            }


            return Ok();


        }

        [HttpGet]
        [Route("GetAllReviews")]
        public IHttpActionResult GetAllReviews()
        {

            List<Review> allReviews = Database.Database.GetAllReviews();
            allReviews = allReviews.Where(x => !x.IsDeleted).ToList();


            List<ReviewDto> results = new List<ReviewDto>();

            foreach (var review in allReviews)
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



        [HttpPut]
        [Route("UpdateOrderStatus")]
        public IHttpActionResult UpdateOrderStatus([FromBody] UpdateOrderStatusInput updateOrderStatusInput)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            List<Order> orders = (List<Order>)HttpContext.Current.Application["orders"];

            Order foundOrder = orders.FirstOrDefault(x => !x.IsDeleted && x.Id == updateOrderStatusInput.Id && x.OrderState == OrderState.Active);

            if(foundOrder == null)
            {
                return Content(HttpStatusCode.Unauthorized, $"There is not active ordet with id = {updateOrderStatusInput.Id}!");
            }


            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            if (updateOrderStatusInput.Status == OrderStatusInputEnum.Cancelled)
            {              
                var productFromOrder = products.FirstOrDefault(x => x.Id == foundOrder.Product.Id);
                productFromOrder.Quantity += foundOrder.Quantity;
                foundOrder.OrderState = OrderState.Canceled;

            }else
            {
                foundOrder.OrderState = OrderState.Completed;
            }


            Database.Database.InsertEntities(orders, Database.Database.ordersPath);
            Database.Database.InsertEntities(products, Database.Database.productsPath);
            HttpContext.Current.Application["orders"] = Database.Database.GetAllOrders();
            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();
            HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

            return Content(HttpStatusCode.OK, foundOrder);

        }



        [HttpDelete]
        [Route("DeleteUser")]
        public IHttpActionResult DeleteUser([FromBody] DeleteSalesmanInput deleteSalesmanInput)
        {

            if (string.IsNullOrEmpty(deleteSalesmanInput.Username))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            Admin admin = (Admin)loggedIn;


            List<User> users = (List<User>)HttpContext.Current.Application["users"];

            var toDelete = users.FirstOrDefault(x => !x.IsDeleted && x.Username == deleteSalesmanInput.Username && (x.Role == Role.Customer || x.Role == Role.Salesman));

            if (toDelete == null)
            {
                return Content(HttpStatusCode.NotFound, $"User with id = {deleteSalesmanInput.Username} not found!");
            }


            if(toDelete.Role == Role.Salesman)
            {

                List<Salesman> salesmans = (List<Salesman>)HttpContext.Current.Application["salesmans"];

                User foundUser = salesmans.FirstOrDefault(x => x.Username == deleteSalesmanInput.Username && !x.IsDeleted);

                if (foundUser == null)
                {
                    return Content(HttpStatusCode.NotFound, $"User with id = {deleteSalesmanInput.Username} not found!");
                }



                foundUser.IsDeleted = true;

                Database.Database.InsertEntities(salesmans, Database.Database.salesmanPath);
                HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();
                HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();


                return Ok(foundUser);
            }
            else
            {
                List<Customer> customers = (List<Customer>)HttpContext.Current.Application["customers"];

                User foundUser = customers.FirstOrDefault(x => x.Username == deleteSalesmanInput.Username && !x.IsDeleted);

                if (foundUser == null)
                {
                    return Content(HttpStatusCode.NotFound, $"User with id = {deleteSalesmanInput.Username} not found!");
                }


                List<Order> allOrders = Database.Database.GetAllOrders();
                List<Order> userOrders = allOrders.Where(x => !x.IsDeleted && x.OrderState == OrderState.Active && x.Customer.Username == foundUser.Username).ToList();

                List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

                if (userOrders.Count() >= 0)
                {
                    

                    foreach(var order in userOrders)
                    {
                        var productFromOrder = products.FirstOrDefault(x => x.Id == order.Product.Id);
                        productFromOrder.Quantity += order.Quantity;

                    }
                }


                List<Order> toUpdateOrders = Database.Database.GetAllOrders();

                foreach(var order in userOrders)
                {
                    var foundOrder = toUpdateOrders.FirstOrDefault(x => x.Id == order.Id);
                    foundOrder.IsDeleted = true;
                }


                foundUser.IsDeleted = true;

                Database.Database.InsertEntities(customers, Database.Database.customersPath);
                Database.Database.InsertEntities(products, Database.Database.productsPath);
                Database.Database.InsertEntities(toUpdateOrders, Database.Database.ordersPath);
                HttpContext.Current.Application["orders"] = Database.Database.GetAllOrders();
                HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
                HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();


                return Ok(foundUser);
            }



        }

        [HttpPut]
        [Route("ApproveReview")]
        public IHttpActionResult ApproveReview([FromBody] ApprovedReviewInput approvedReviewInput)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }

            List<Review> allReviews = Database.Database.GetAllReviews();
            var foundReview = allReviews.FirstOrDefault(x => !x.IsDeleted && x.Id == approvedReviewInput.Id);

            if(foundReview == null)
            {
                return Content(HttpStatusCode.NotFound, $"Review with id = {approvedReviewInput.Id} not found!");
            }

            foundReview.IsApproved = true;


            var res = new ReviewDto()
            {
                Id = foundReview.Id,
                Content = foundReview.Content,
                ImageURL = foundReview.ImageURL,
                ProductId = foundReview.Product.Id,
                ProductName = foundReview.Product.Name,
                ReviewerUsername = foundReview.Reviewer.Username,
                Title = foundReview.Title,
                IsApproved = foundReview.IsApproved
                
            };

            Database.Database.InsertEntities(allReviews, Database.Database.reviewsPath);
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();




            return Ok(res);

        }


        [HttpPut]
        [Route("DenyReview")]
        public IHttpActionResult DenyReview([FromBody] DenyReviewInput denyReviewInput)
        {
            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Admin)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            List<Review> allReviews = Database.Database.GetAllReviews();
            var foundReview = allReviews.FirstOrDefault(x => !x.IsDeleted && x.Id == denyReviewInput.Id);

            if (foundReview == null)
            {
                return Content(HttpStatusCode.NotFound, $"Review with id = {denyReviewInput.Id} not found!");
            }

            foundReview.IsApproved = false;


            var res = new ReviewDto()
            {
                Id = foundReview.Id,
                Content = foundReview.Content,
                ImageURL = foundReview.ImageURL,
                ProductId = foundReview.Product.Id,
                ProductName = foundReview.Product.Name,
                ReviewerUsername = foundReview.Reviewer.Username,
                Title = foundReview.Title,
                IsApproved = foundReview.IsApproved

            };

            Database.Database.InsertEntities(allReviews, Database.Database.reviewsPath);
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();




            return Ok(res);

        }








        private List<User> SortProductsByField(List<User> products, SortUserBy field, SortOrder sortOrder)
        {
            if (field == SortUserBy.Name)
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.Name).ToList() : products.OrderByDescending(x => x.Name).ToList();
            }
            else if (field == SortUserBy.Role)
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.Role).ToList() : products.OrderByDescending(x => x.Role).ToList();
            }
            else
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.BirthDate).ToList() : products.OrderByDescending(x => x.BirthDate).ToList();
            }
        }

        private IHttpActionResult ValidateRegisterInputValid(BaseUserInput baseUserInput)
        {
            if (string.IsNullOrEmpty(baseUserInput.Username))
            {
                return Content(HttpStatusCode.BadRequest, "Empty username!");
            }

            if (string.IsNullOrEmpty(baseUserInput.Password))
            {
                return Content(HttpStatusCode.BadRequest, "Empty password!");
            }


            if (string.IsNullOrEmpty(baseUserInput.ConfirmPassword))
            {
                return Content(HttpStatusCode.BadRequest, "Empty confirm password!");
            }


            if (baseUserInput.Password != baseUserInput.ConfirmPassword)
            {
                return Content(HttpStatusCode.BadRequest, "Passwords are not matching!");
            }

            if (string.IsNullOrEmpty(baseUserInput.Email))
            {
                return Content(HttpStatusCode.BadRequest, "Empty email!");
            }

            if (baseUserInput.DateOfBirth == DateTime.MinValue || baseUserInput.DateOfBirth >= DateTime.Now)
            {
                return Content(HttpStatusCode.BadRequest, "Date of birth has to be in past!");
            }

            return null;

        }


        public bool IsDateValid(DateTime dateFom, DateTime dateTo)
        {
            if(dateFom >= dateTo)
            {
                return false;
            }

            return true;
        }
    }
}
