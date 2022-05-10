using System;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using BIZ.ExternalIntegration.ASP.MVC;
using BIZ.ExternalIntegration.ASP.MVC.Dependencies;
using BIZ.ExternalIntegration.Common;

namespace ProjectWebApi
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
            
            BIZApplicationInitializer.InitializeEnvironment(new DependencyResolverConfigMVC(), 300, true);
            var eLeedLogin = "admin";
            var eLeedPasswordHash = "47DEQpj8HBSa+/TImW+5JCeuQeRkm5NMpJWZG3hSuFU=";
            BIZApplicationInitializer.RegisterGlobalSession(BIZAuthInfo.CreateWithHash(eLeedLogin, eLeedPasswordHash, null), true);

            DependencyResolverConfig1.Configure();
        }

        protected void Application_End(object sender, EventArgs e)
        {
            BIZApplicationInitializer.RemoveGlobalSession();
            BIZApplicationInitializer.CleanEnvironment();
        }

        protected void Session_Start(object sender, EventArgs e)
        {

        }

        protected void Application_AcquireRequestState(object sender, EventArgs e)
        {
            BIZApplicationInitializer.SetThreadCurrentCulture();
        }

        protected void Session_End(object sender, EventArgs e)
        {

        }
    }
}
