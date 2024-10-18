using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using WebShop.Models.Domain;
using WebShop.Models.Enums;

namespace WebShop
{
    public class WebApiApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);


            HttpContext.Current.Application["users"] = Database.Database.GetAllUsers();
            HttpContext.Current.Application["customers"] = Database.Database.ReadCustomerEntites();
            HttpContext.Current.Application["salesmans"] = Database.Database.ReadSalesmanEntites();
            HttpContext.Current.Application["admins"] = Database.Database.ReadAdminEntites();
            HttpContext.Current.Application["products"] = Database.Database.ReadProductEntites(); ;
            HttpContext.Current.Application["orders"] = Database.Database.GetAllOrders();

        }

        protected void Application_PostAuthorizeRequest()
        {
            System.Web.HttpContext.Current.SetSessionStateBehavior(System.Web.SessionState.SessionStateBehavior.Required);
        }
    }
}
