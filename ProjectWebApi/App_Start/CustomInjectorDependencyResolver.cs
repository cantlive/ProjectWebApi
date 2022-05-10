using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Http.Dependencies;
using SimpleInjector;
using SimpleInjector.Integration.Web.Mvc;

namespace ProjectWebApi
{
    public class CustomInjectorDependencyResolver : SimpleInjectorDependencyResolver, IDependencyResolver
    {
        public CustomInjectorDependencyResolver(Container container)
            : base(container)
        {
            if (container == null)
            {
                throw new ArgumentNullException(nameof(container));
            }
        }

        IDependencyScope IDependencyResolver.BeginScope()
        {
            return this;
        }

        object IDependencyScope.GetService(Type serviceType)
        {
            return ((IServiceProvider)Container)
                .GetService(serviceType);
        }

        IEnumerable<object> IDependencyScope.GetServices(Type serviceType)
        {
            IServiceProvider provider = Container;
            Type collectionType = typeof(IEnumerable<>).MakeGenericType(serviceType);
            var services = (IEnumerable<object>)provider.GetService(collectionType);
            return services ?? Enumerable.Empty<object>();
        }

        void IDisposable.Dispose()
        {
        }
    }
}