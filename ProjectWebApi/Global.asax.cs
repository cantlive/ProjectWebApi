using System;
using System.Configuration;
using System.Web;
using System.Web.Http;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.SessionState;
using BIZ.ExternalIntegration.ASP.MVC;
using BIZ.ExternalIntegration.ASP.MVC.Dependencies;
using BIZ.ExternalIntegration.Common;

namespace ProjectWebApi
{
    public class MvcApplication : HttpApplication
    {
        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            GlobalConfiguration.Configure(WebApiConfig.Register);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);

            #region eLeed Environment initialization (auto genereated code)

            BIZApplicationInitializer.InitializeEnvironment(new DependencyResolverConfigMVC(), 300, true);
            BIZApplicationInitializer.RegisterDefaultAuthentificationPage("/Home/Index");

            string login = ConfigurationManager.AppSettings["EleedLogin"];
            string passwordHash = ConfigurationManager.AppSettings["EleedPasswordHash"];
            var authInfo = BIZAuthInfo.CreateWithHash(login, passwordHash);
            BIZApplicationInitializer.RunAssembliesUpdater(authInfo);

            #endregion
        }

        protected void Application_End(object sender, EventArgs e)
        {
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
            BIZApplicationInitializer.RemoveLocalSession();
        }

        protected void Application_PostAuthorizeRequest()
        {
            if (IsWebApiRequest())
                HttpContext.Current.SetSessionStateBehavior(SessionStateBehavior.Required);
        }

        private bool IsWebApiRequest()
        {
            return HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath != null && HttpContext.Current.Request.AppRelativeCurrentExecutionFilePath.StartsWith(WebApiConfig.UrlPrefixRelative);
        }
    }
}
