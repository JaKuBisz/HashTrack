using System;
using System.IO;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Handlers;

namespace HashTrack.IoC
{
    public static class Startup
    {
        public static IContainer Container { get; private set; }
        public static ServiceLocator ServiceLocator { get; private set; }

        public static void ConfigureContainer(Action<ContainerBuilder> additionalConfigurations = null)
        {
            var builder = new ContainerBuilder();


            RegisterServices(builder, GetAssemblies());
            // Register services here, for example:
            //builder.RegisterType<AdvancedSearchCompleteHandler>().As<AdvancedSearchCompleteHandler>().SingleInstance();
            additionalConfigurations?.Invoke(builder);


            Container = builder.Build();

            var scope = Container.BeginLifetimeScope();
            ServiceLocator = new ServiceLocator(scope);
        }
        
        private static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
            return new Assembly[]
            {
                Assembly.Load("HashTrack.Core"),
                Assembly.Load("HashTrack.BusinessLogic"),
                Assembly.Load("HashTrack.Persistence"),
                Assembly.Load("HashTrack.IoC"),
                Assembly.Load("HashTrack")
            };
        }

        private static void RegisterServices(ContainerBuilder builder, Assembly[] assemblies)
        {
            Console.WriteLine("Registering services for assemblies: " + string.Join(", ", assemblies.Select(a => a.GetName().Name)));

            // Register standard services
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t =>
                {
                    var attribute = t.GetCustomAttribute<RegisterServiceAttribute>();
                    return attribute != null;
                })
                .As(t =>
                {
                    var attribute = t.GetCustomAttribute<RegisterServiceAttribute>();
                    if (attribute.ServiceType != null)
                        return new[] { attribute.ServiceType };
                    return t.GetInterfaces().Where(i => i.Name == $"I{t.Name}");
                })
                .PropertiesAutowired()
                .ConfigureLifecycle();

            // Register keyed services
            builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetCustomAttribute<RegisterHandlerAttribute>() != null)
                .Keyed<ISearchCompleteHandler>(t => t.GetCustomAttribute<RegisterHandlerAttribute>().Key)
                .PropertiesAutowired()
                .ConfigureLifecycle();
        }

        private static IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> ConfigureLifecycle<TLimit, TReflectionActivatorData, TStyle>(
            this IRegistrationBuilder<TLimit, TReflectionActivatorData, TStyle> registrationBuilder)
            where TReflectionActivatorData : ReflectionActivatorData
        {
            var attribute = registrationBuilder.ActivatorData.ImplementationType
                .GetCustomAttribute<RegisterServiceAttribute>();
            if (attribute != null)
            {
                switch (attribute.LifeCycle)
                {
                    case LifeCycle.Singleton:
                        registrationBuilder.SingleInstance();
                        break;
                    case LifeCycle.Scoped:
                        registrationBuilder.InstancePerLifetimeScope();
                        break;
                    case LifeCycle.Transient:
                        registrationBuilder.InstancePerDependency();
                        break;
                }
            }

            return registrationBuilder;
        }
    }

}
