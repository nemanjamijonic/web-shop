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
    [RoutePrefix("salesmans")]
    public class SalesmanController : ApiController
    {

        [HttpGet]
        [Route("GetProducts")]
        public IHttpActionResult GetProducts()
        {


            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Salesman)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> ownedProducts = Database.Database.GetOwnedProducts(loggedIn.Username);
            List<Product> products = Database.Database.ReadProductEntites();



            List<Product> availableOwnedProduct = new List<Product>();

            foreach (var product in ownedProducts)
            {
                var foundProduct = products.FirstOrDefault(x => x.Id == product.Id);

                if (foundProduct != null)
                {
                    availableOwnedProduct.Add(foundProduct);
                }
            }

            var res = availableOwnedProduct.Where(x => !x.IsDeleted).ToList();
            return Ok(res);

        }


        [HttpPost]
        [Route("Sort")]
        public IHttpActionResult Sort([FromBody] SortProductDto sortInput)
        {

            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Salesman)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            List<Product> ownedProducts = Database.Database.GetOwnedProducts(loggedIn.Username);
            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];


            List<Product> availableOwnedProduct = new List<Product>();

            foreach (var product in ownedProducts)
            {
                var foundProduct = products.FirstOrDefault(x => x.Id == product.Id);

                if (foundProduct != null)
                {
                    availableOwnedProduct.Add(foundProduct);
                }
            }


            var results = SortProductsByField(availableOwnedProduct, sortInput.SortBy, sortInput.SortOrder);
            results = results.Where(x => !x.IsDeleted).ToList();

            return Ok(results);
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

            if (loggedIn.Role == Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> ownedProducts = Database.Database.GetOwnedProducts(loggedIn.Username);
            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];


            List<Product> availableOwnedProduct = new List<Product>();

            foreach (var product in ownedProducts)
            {
                var foundProduct = products.FirstOrDefault(x => x.Id == product.Id);

                if (foundProduct != null)
                {
                    availableOwnedProduct.Add(foundProduct);
                }
            }


            if(filter.Filter == ProductFilterType.All)
            {
                return Ok(availableOwnedProduct);
            }
            else if(filter.Filter == ProductFilterType.Available)
            {
                var res =  availableOwnedProduct.Where(x => x.IsAvaliable);
                res = res.Where(x => !x.IsDeleted).ToList();

                return Ok(res);
            }
            else
            {
                var res = availableOwnedProduct.Where(x => !x.IsAvaliable);
                res = res.Where(x => !x.IsDeleted).ToList();
                return Ok(res);
            }

        }

        [HttpPost]
        [Route("CreateProduct")]
        public IHttpActionResult CreateProduct([FromBody] CreateProductInput createProductInput)
        {

            if (string.IsNullOrEmpty(createProductInput.City))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }



            if (string.IsNullOrEmpty(createProductInput.Description))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            if (string.IsNullOrEmpty(createProductInput.ImageURL))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            if (string.IsNullOrEmpty(createProductInput.Name))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if (createProductInput.Price <= 0)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if(createProductInput.Quantity <= 0 )
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }





            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role != Role.Salesman)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            Salesman salesman = (Salesman)loggedIn;


            Product product = Product.CreateNewInstance(createProductInput);
            products.Add(product);

            ProductOwner productOwner = new ProductOwner()
            {
                Product = product,
                Owner = salesman.Username
            };

            Database.Database.InsertEntity(product, Database.Database.productsPath);
            Database.Database.InsertEntity(productOwner, Database.Database.productOwnerPath);

            HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();


            return Ok(product);


        }


        [HttpPut]
        [Route("EditProduct")]
        public IHttpActionResult EditProduct([FromBody] EditProductInput editProductInput)
        {


            if (string.IsNullOrEmpty(editProductInput.City))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }



            if (string.IsNullOrEmpty(editProductInput.Description))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            if (string.IsNullOrEmpty(editProductInput.ImageURL))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            if (string.IsNullOrEmpty(editProductInput.Name))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if (editProductInput.Price <= 0)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            if (editProductInput.Quantity <= 0)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role == Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }



            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];


            Product foundProduct = products.FirstOrDefault(x => x.Id == editProductInput.Id);

            if(foundProduct == null)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {editProductInput.Id} not found!");
            }

            if(!foundProduct.IsAvaliable)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {editProductInput.Id} is not available!");
            }

            if (foundProduct.IsDeleted)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {editProductInput.Id} is deleted!");
            }


            foundProduct.UpdateProduct(editProductInput);


            Database.Database.InsertEntities(products, Database.Database.productsPath);

            HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();


            return Ok(loggedIn);


        }

        [HttpDelete]
        [Route("DeleteProduct")]
        public IHttpActionResult DeleteProduct([FromBody] DeleteProductInput deleteProductInput)
        {

            if (deleteProductInput.Id == Guid.Empty)
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }


            User loggedIn = (User)HttpContext.Current.Session["user"];
            if (loggedIn == null)
            {
                return Content(HttpStatusCode.Unauthorized, "Please login!");
            }

            if (loggedIn.Role == Role.Customer)
            {
                return Content(HttpStatusCode.Unauthorized, "You are not authorized!");
            }


            //Salesman salesman = (Salesman)loggedIn;

            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            Product foundProduct = products.FirstOrDefault(x => x.Id == deleteProductInput.Id);

            if(foundProduct == null)
            {
                return Content(HttpStatusCode.NotFound, $"Product with id = {deleteProductInput.Id} not found!");
            }

            if (foundProduct.IsDeleted)
            {
                return Content(HttpStatusCode.BadRequest, $"Product with id {deleteProductInput.Id} is deleted!");
            }



            foundProduct.IsDeleted = true;

            Database.Database.InsertEntities(products, Database.Database.productsPath);
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites();
            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();


            return Ok(foundProduct);



        }


        private List<Product> SortProductsByField(List<Product> products, SortBy field, SortOrder sortOrder)
        {
            if (field == SortBy.Name)
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.Name).ToList() : products.OrderByDescending(x => x.Name).ToList();
            }
            else if (field == SortBy.Price)
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.Price).ToList() : products.OrderByDescending(x => x.Price).ToList();
            }
            else
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.PostDate).ToList() : products.OrderByDescending(x => x.PostDate).ToList();
            }
        }
    }
}
