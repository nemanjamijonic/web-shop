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
    [RoutePrefix("users")]
    public class LoginRegisterController : ApiController
    {
        [HttpPost]
        [Route("Login")]
        public IHttpActionResult Login([FromBody] LoginUser loginParameters)
        {

            if(!IsLoginInputValid(loginParameters))
            {
                return Content(HttpStatusCode.BadRequest, "Invalid input");
            }

            List<User> users = (List<User>)HttpContext.Current.Application["users"];

            User loggedUser = users.FirstOrDefault(x => x.Username.Equals(loginParameters.Username) && x.Password.Equals(loginParameters.Password) && !x.IsDeleted);
            if (loggedUser == null)
            {
                return Content(HttpStatusCode.BadRequest, "Unsucessfull login");
            }

            HttpContext.Current.Session["user"] = loggedUser;

            return Ok(loggedUser);

        }

        [HttpPost]
        [Route("Logout")]
        public IHttpActionResult Logout()
        {
            HttpContext.Current.Session["user"] = null;
            return Ok();
        }


        [HttpGet]
        [Route("GetLoggedinUser")]
        public IHttpActionResult GetLoggedinUser()
        {
            User user = (User)HttpContext.Current.Session["user"];

            return Ok(user);

        }


        [HttpPost]
        [Route("Register")]
        public IHttpActionResult Register([FromBody] RegisterUser registerParameters)
        {

            var validation = ValidateRegisterInputValid(registerParameters);

            if (validation != null)
                return validation;

            List<User> list = (List<User>)HttpContext.Current.Application["users"];

            if(list.Any(x => x.Username == registerParameters.Username))
            {
                return Content(HttpStatusCode.BadRequest, "User with this username already exists!");
            }

            Customer customer = Customer.CreateNewInstance(registerParameters);

            Database.Database.InsertEntity(customer, Database.Database.customersPath);
            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();

            return Content(HttpStatusCode.Created, customer);

        }


        [HttpPost]
        [Route("UpdateProfile")]
        public IHttpActionResult UpdateProfile([FromBody] UpdateUserProfile updateUserProfile)
        {

            var validation = ValidateRegisterInputValid(updateUserProfile);

            if (validation != null)
                return validation;

            List<User> users = (List<User>)HttpContext.Current.Application["users"];

            var foundUser = users.FirstOrDefault(x => x.Username == updateUserProfile.Username);

            if(foundUser == null)
            {
                return Content(HttpStatusCode.NotFound, $"User {updateUserProfile.Username} not found!");
            }

            Role role = foundUser.Role;

            if(role == Role.Customer)
            {
                List<Customer> customers = (List<Customer>)HttpContext.Current.Application["customers"];
                Customer customer = customers.First(x => x.Username == updateUserProfile.Username);
                customer.UpdateProfile(updateUserProfile);

                Database.Database.InsertEntities(customers, Database.Database.customersPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();
                
                HttpContext.Current.Session["user"] = (User)customer;


                return Content(HttpStatusCode.OK, customer);

            }

            if (role == Role.Admin)
            {
                List<Admin> admins = (List<Admin>)HttpContext.Current.Application["admins"];
                Admin admin = admins.First(x => x.Username == updateUserProfile.Username);
                admin.UpdateProfile(updateUserProfile);

                Database.Database.InsertEntities(admins, Database.Database.adminsPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();


                HttpContext.Current.Session["user"] = (User)admin;

                return Content(HttpStatusCode.OK, admin);

            }

            if (role == Role.Salesman)
            {
                List<Salesman> salesmans = (List<Salesman>)HttpContext.Current.Application["salesmans"];
                Salesman salesman = salesmans.First(x => x.Username == updateUserProfile.Username);
                salesman.UpdateProfile(updateUserProfile);

                Database.Database.InsertEntities(salesmans, Database.Database.salesmanPath);
                HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();


                HttpContext.Current.Session["user"] = (User)salesman;

                return Content(HttpStatusCode.OK, salesman);

            }


            return Ok();

        }

        private IHttpActionResult ValidateRegisterInputValid(BaseUserInput baseUserInput)
        {
            if(string.IsNullOrEmpty(baseUserInput.Username))
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
            

            if(baseUserInput.Password != baseUserInput.ConfirmPassword)
            {
                return Content(HttpStatusCode.BadRequest, "Passwords are not matching!");
            }

            if (string.IsNullOrEmpty(baseUserInput.Email))
            {
                return Content(HttpStatusCode.BadRequest, "Empty email!");
            }

            if(baseUserInput.DateOfBirth == DateTime.MinValue || baseUserInput.DateOfBirth >= DateTime.Now)
            {
                return Content(HttpStatusCode.BadRequest, "Date of birth has to be in past!");
            }

            return null;

        }

        private bool IsLoginInputValid(LoginUser loginParameters)
        {
            return !string.IsNullOrEmpty(loginParameters.Username) && !string.IsNullOrEmpty(loginParameters.Password);
        }



    }
}
