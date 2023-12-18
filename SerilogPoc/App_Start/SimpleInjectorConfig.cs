using SerilogPoc.Contracts;
using SerilogPoc.Services;
using SimpleInjector;
using SimpleInjector.Advanced;
using SimpleInjector.Integration.Web;
using SimpleInjector.Integration.Web.Mvc;
using System;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;

namespace SerilogPoc
{
    public static class SimpleInjectorConfig
    {
        public static void RegisterComponents()
        {
            var container = new Container();
            container.Options.DefaultScopedLifestyle = new WebRequestLifestyle();
            container.Options.ConstructorResolutionBehavior = new GreediestConstructorBehavior();

            RegisterDependencies(container);

            container.RegisterMvcControllers(Assembly.GetExecutingAssembly());

            container.Verify();

            DependencyResolver.SetResolver(new SimpleInjectorDependencyResolver(container));
        }

        private static void RegisterDependencies(Container container)
        {
            container.Register<ILogService, LogService>(Lifestyle.Singleton);
        }
    }

    public class GreediestConstructorBehavior : IConstructorResolutionBehavior
    {
        // Ref - https://docs.simpleinjector.org/en/latest/extensibility.html#overriding-constructor-resolution-behavior

        public ConstructorInfo TryGetConstructor(Type implementationType, out string errorMessage)
        {
            errorMessage = $"{implementationType} has no public constructors.";

            return (from ctor in implementationType.GetConstructors()
                    orderby ctor.GetParameters().Length descending
                    select ctor)
                .FirstOrDefault();
        }
    }
}