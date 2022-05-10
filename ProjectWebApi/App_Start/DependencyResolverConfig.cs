using System.Web.Http;
using System.Web.Mvc;
using ProjectWebApi.DataAccess.Repositories;
using SimpleInjector;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.WebApi;

namespace ProjectWebApi
{
    public class DependencyResolverConfig1
    {
        public static void Configure()
        {
            var container = CreateContainer(new WebRequestLifestyle());

            DependencyResolver.SetResolver(
                new CustomInjectorDependencyResolver(container));

            GlobalConfiguration.Configuration.DependencyResolver =
                new CustomInjectorDependencyResolver(container);

            GlobalConfiguration.Configuration.DependencyResolver =
                new SimpleInjectorWebApiDependencyResolver(container);
        }

        private static Container CreateContainer(ScopedLifestyle defaultLifestyle)
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = defaultLifestyle;

            // Repositories
            container.Register<IDictionaryRepository1, DictionaryRepository1>(Lifestyle.Transient);

            // MVC Controllers
            container.RegisterMvcControllers();

            // WebApi Controllers
            container.RegisterWebApiControllers(GlobalConfiguration.Configuration);

            container.Verify();

            return container;
        }
    }
}