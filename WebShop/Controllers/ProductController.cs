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
    [RoutePrefix("products")]
    public class ProductController : ApiController
    {
        [HttpGet]
        [Route("GetProducts")]
        public List<Product> GetProducts()
        {

            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];
            return products.Where(x => !x.IsDeleted).OrderByDescending(p => p.PostDate).ToList(); ;

        }

        [HttpGet]
        [Route("GetById")]
        public IHttpActionResult GetById([FromUri] Guid productId)
        {
            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];
            var res = products.FirstOrDefault(x => x.Id == productId && !x.IsDeleted);
            return Ok(res);
        }

        [HttpPost]
        [Route("Search")]
        public List<Product> Search([FromBody] SearchProductDto searchInput)
        {
            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];
            List<Product> results = new List<Product>();

            products = products.Where(x => !x.IsDeleted).ToList();


            if(searchInput.IsCombined)
            {

                if (!string.IsNullOrEmpty(searchInput.Name))
                {
                    results = products.Where(x => x.Name.ToLower().Contains(searchInput.Name.ToLower())).ToList();
                }

                if (!string.IsNullOrEmpty(searchInput.City))
                {
                    results = products.Where(x => x.City.ToLower().Contains(searchInput.City.ToLower())).ToList();
                }

                if (IsPriceValid(searchInput))
                {
                    results = products.Where(x => x.Price >= searchInput.MinPrice && x.Price <= searchInput.MaxPrice).ToList();
                }

                return results;

            }

             return products.Where(x => (!string.IsNullOrEmpty(searchInput.Name) && x.Name.ToLower().Contains(searchInput.Name.ToLower()))
                                        || (!string.IsNullOrEmpty(searchInput.City) && x.City.ToLower().Contains(searchInput.City.ToLower()))
                                        || (IsPriceValid(searchInput) && x.Price >= searchInput.MinPrice && x.Price <= searchInput.MaxPrice)).ToList();

        }



        [HttpPost]
        [Route("Sort")]
        public List<Product> Sort([FromBody] SortProductDto sortInput)
        {
             
            List<Product> products = (List<Product>)HttpContext.Current.Application["products"];

            products = products.Where(x => !x.IsDeleted).ToList();

            return SortProductsByField(products, sortInput.SortBy, sortInput.SortOrder);

        }

        
        private bool IsPriceValid(SearchProductDto searchInput) => searchInput.MinPrice < searchInput.MaxPrice && searchInput.MaxPrice >= 0 && searchInput.MinPrice >= 0 && searchInput.MaxPrice != double.MaxValue;
        private List<Product> SortProductsByField(List<Product> products, SortBy field, SortOrder sortOrder)
        {
            if(field == SortBy.Name)
            {
                return sortOrder == SortOrder.Asc ? products.OrderBy(x => x.Name).ToList() : products.OrderByDescending(x => x.Name).ToList();
            }
            else if(field == SortBy.Price)
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
