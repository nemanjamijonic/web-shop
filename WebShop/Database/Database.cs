using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Hosting;
using WebShop.Handlers;
using WebShop.Models.DatabaseModels;
using WebShop.Models.Domain;
using WebShop.Models.Enums;

namespace WebShop.Database
{
    public class Database
    {

        public const int CustomerFieldsInFile = 9;
        public const int SalesmanFieldsInFile = 9;
        public const int AdminFieldsInFile = 9;
        public const int ProductsFieldsInFile = 10;
        public const int FavouriteProductsFieldsInFile = 11;
        public const int ProductOwnerFieldsInFile = 11;
        public const int OrdersFieldsInFile = 24;
        public const int ReviewFieldsInFile = 25;

        public const string customersPath = "~/App_Data/Customers.txt";
        public const string salesmanPath = "~/App_Data/Salesmans.txt";
        public const string adminsPath = "~/App_Data/Admins.txt";
        public const string productsPath = "~/App_Data/Products.txt";
        public const string favouriteProductsPath = "~/App_Data/Favourites.txt";
        public const string productOwnerPath = "~/App_Data/ProductOwners.txt";
        public const string ordersPath = "~/App_Data/Orders.txt";
        public const string reviewsPath = "~/App_Data/Reviews.txt";


        public static List<User> GetAllUsers()
        {
            List<Customer> customers = ReadCustomerEntites();
            List<Salesman> salesmans = ReadSalesmanEntites();
            List<Admin> admins = ReadAdminEntites();

            List<User> users = new List<User>();

            users.AddRange(customers.Cast<User>());
            users.AddRange(salesmans.Cast<User>());
            users.AddRange(admins.Cast<User>());

            return users;

        }


        public static List<Customer> ReadCustomerEntites()
        {

            List<Customer> customers = new List<Customer>();
            var lines = TextFileHandler.ReadTextFile(customersPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != CustomerFieldsInFile)
                    continue;

                var customer = new Customer(values[0], values[1], values[2], values[3], (Gender)Enum.Parse(typeof(Gender), values[4]), values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[8]));
                customer.FavouriteProducts = GetUserFavouriteProducts(customer.Username);
                customer.Orders = GetUserOrders(customer.Username);
                customers.Add(customer);

            }

            return customers;
        }

        public static List<Salesman> ReadSalesmanEntites()
        {

            List<Salesman> salesmans = new List<Salesman>();
            var lines = TextFileHandler.ReadTextFile(salesmanPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != SalesmanFieldsInFile)
                    continue;

                var salesman = new Salesman(values[0], values[1], values[2], values[3], (Gender)Enum.Parse(typeof(Gender), values[4]), values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[8]));
                salesman.PublishedProducts = GetOwnedProducts(salesman.Username);
                salesmans.Add(salesman);


            }

            return salesmans;

        }

        public static List<Admin> ReadAdminEntites()
        {

            List<Admin> admins = new List<Admin>();
            var lines = TextFileHandler.ReadTextFile(adminsPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != AdminFieldsInFile)
                    continue;

                admins.Add(
                    new Admin(values[0], values[1], values[2], values[3], (Gender)Enum.Parse(typeof(Gender), values[4]), values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[8]))
                );

            }

            return admins;
        }

        public static List<Product> ReadProductEntites()
        {

            List<Product> products = new List<Product>();
            var lines = TextFileHandler.ReadTextFile(productsPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != ProductsFieldsInFile)
                    continue;


                var product = new Product(Guid.Parse(values[0]), values[1], Double.Parse(values[2]), int.Parse(values[3]), values[4], values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[7], bool.Parse(values[8]), bool.Parse(values[9]));
                product.Reviews = GetProductReviews(product.Id);

                products.Add(product);


            }

            return products;
        }


        public static List<Product> GetUserFavouriteProducts(string username)
        {
            List<Product> products = new List<Product>();
            var lines = TextFileHandler.ReadTextFile(favouriteProductsPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != FavouriteProductsFieldsInFile)
                    continue;

                var favouriteProduct = new FavouriteProduct(Guid.Parse(values[0]), values[1], Double.Parse(values[2]), int.Parse(values[3]), values[4], values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[7], bool.Parse(values[8]), bool.Parse(values[9]), values[10]);

                if (favouriteProduct.Username != username)
                    continue;

                products.Add(favouriteProduct.Product);

            }

            return products;

        }

        public static List<Product> GetOwnedProducts(string username)
        {
            List<Product> products = new List<Product>();
            var lines = TextFileHandler.ReadTextFile(productOwnerPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != ProductOwnerFieldsInFile)
                    continue;

                var productOwner = new ProductOwner(Guid.Parse(values[0]), values[1], Double.Parse(values[2]), int.Parse(values[3]), values[4], values[5], DateTime.ParseExact(values[6], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[7], bool.Parse(values[8]), bool.Parse(values[9]), values[10]);

                if (productOwner.Owner != username)
                    continue;

                products.Add(productOwner.Product);

            }

            return products;

        }


        public static List<Order> GetUserOrders(string username)
        {
            List<Order> orders = new List<Order>();
            var lines = TextFileHandler.ReadTextFile(ordersPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != OrdersFieldsInFile)
                    continue;

                var product = new Product(Guid.Parse(values[1]), values[2], Double.Parse(values[3]), int.Parse(values[4]), values[5], values[6], DateTime.ParseExact(values[7], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[8], bool.Parse(values[9]), bool.Parse(values[10]));
                var orderQuantity = int.Parse(values[11]);
                var customer = new Customer(values[12], values[13], values[14], values[15], (Gender)Enum.Parse(typeof(Gender), values[16]), values[17], DateTime.ParseExact(values[18], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[20]));

                var order = new Order()
                {
                    Id = Guid.Parse(values[0]),
                    Product = product,
                    Customer = customer,
                    Quantity = orderQuantity,
                    OrderDate = DateTime.ParseExact(values[21], "dd-MM-yyyy", CultureInfo.CurrentCulture),
                    OrderState = (OrderState)Enum.Parse(typeof(OrderState), values[22]),
                    IsDeleted = bool.Parse(values[23])
                };



                if (order.Customer.Username != username)
                    continue;

                if (order.IsDeleted)
                    continue;

                orders.Add(order);

            }

            return orders;

        }


        public static List<Order> GetAllOrders()
        {
            List<Order> orders = new List<Order>();
            var lines = TextFileHandler.ReadTextFile(ordersPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != OrdersFieldsInFile)
                    continue;

                var product = new Product(Guid.Parse(values[1]), values[2], Double.Parse(values[3]), int.Parse(values[4]), values[5], values[6], DateTime.ParseExact(values[7], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[8], bool.Parse(values[9]), bool.Parse(values[10]));
                var orderQuantity = int.Parse(values[11]);
                var customer = new Customer(values[12], values[13], values[14], values[15], (Gender)Enum.Parse(typeof(Gender), values[16]), values[17], DateTime.ParseExact(values[18], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[20]));

                var order = new Order()
                {
                    Id = Guid.Parse(values[0]),
                    Product = product,
                    Customer = customer,
                    Quantity = orderQuantity,
                    OrderDate = DateTime.ParseExact(values[21], "dd-MM-yyyy", CultureInfo.CurrentCulture),
                    OrderState = (OrderState)Enum.Parse(typeof(OrderState), values[22]),
                    IsDeleted =  bool.Parse(values[23])
                };

                if (order.IsDeleted)
                    continue;

                orders.Add(order);

            }

            return orders;

        }

        public static List<Review> GetProductReviews(Guid productId)
        {
            List<Review> reviews = new List<Review>();
            var lines = TextFileHandler.ReadTextFile(reviewsPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != ReviewFieldsInFile)
                    continue;

                var product = new Product(Guid.Parse(values[1]), values[2], Double.Parse(values[3]), int.Parse(values[4]), values[5], values[6], DateTime.ParseExact(values[7], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[8], bool.Parse(values[9]), bool.Parse(values[10]));
                var reviewer = new Customer(values[11], values[12], values[13], values[14], (Gender)Enum.Parse(typeof(Gender), values[15]), values[16], DateTime.ParseExact(values[17], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[19]));

               
                var review = new Review()
                {
                    Id = Guid.Parse(values[0]),
                    Product = product,
                    Reviewer = reviewer,
                    Title = values[20],
                    Content = values[21],
                    ImageURL = values[22],
                    IsDeleted = bool.Parse(values[23]),
                    IsApproved = bool.Parse(values[24])
                    
                };



                if (review.Product.Id != productId)
                    continue;

                if (review.IsDeleted || !review.IsApproved)
                    continue;

                reviews.Add(review);

            }

            return reviews;
        }

        public static List<Review> GetAllReviews()
        {
            List<Review> reviews = new List<Review>();
            var lines = TextFileHandler.ReadTextFile(reviewsPath);

            foreach (var line in lines)
            {
                string[] values = line.Split('|');

                if (values.Count() != ReviewFieldsInFile)
                    continue;

                var product = new Product(Guid.Parse(values[1]), values[2], Double.Parse(values[3]), int.Parse(values[4]), values[5], values[6], DateTime.ParseExact(values[7], "dd-MM-yyyy", CultureInfo.CurrentCulture), values[8], bool.Parse(values[9]), bool.Parse(values[10]));
                var reviewer = new Customer(values[11], values[12], values[13], values[14], (Gender)Enum.Parse(typeof(Gender), values[15]), values[16], DateTime.ParseExact(values[17], "dd-MM-yyyy", CultureInfo.CurrentCulture), bool.Parse(values[19]));


                var review = new Review()
                {
                    Id = Guid.Parse(values[0]),
                    Product = product,
                    Reviewer = reviewer,
                    Title = values[20],
                    Content = values[21],
                    ImageURL = values[22],
                    IsDeleted = bool.Parse(values[23]),
                    IsApproved = bool.Parse(values[24])

                };


                reviews.Add(review);

            }

            return reviews;
        }


        public static void InsertEntity<T>(T entity, string path) where T : class
        {
            if (entity != null)
            {
                TextFileHandler.InsertEntityIntoFile(entity, path);
            }
        }

        public static void InsertEntities<T>(List<T> entities, string path) where T : class
        {
            if (entities != null && entities.Any())
            {
                TextFileHandler.InsertEntitiesIntoFile(entities, path);
            }
        }


    }
}