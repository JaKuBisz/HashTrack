using System;
using System.Linq;
using System.Reflection;
using Autofac;
using Autofac.Builder;
using Autofac.Features.Scanning;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Handlers;
using Microsoft.Extensions.Caching.Memory;

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
            builder.Register(c =>
            {
                var options = new MemoryCacheOptions
                {
                    //SizeLimit = 1024,
                    CompactionPercentage = 0.2,
                    ExpirationScanFrequency = TimeSpan.FromMinutes(5)
                }; // You can configure options here
                return new MemoryCache(options);
            }).As<IMemoryCache>().SingleInstance();

            Container = builder.Build();

            var scope = Container.BeginLifetimeScope();
            ServiceLocator = new ServiceLocator(scope);
        }

        private static Assembly[] GetAssemblies()
        {
            return AppDomain.CurrentDomain.GetAssemblies();
            return new[]
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
            Console.WriteLine("Registering services for assemblies: " +
                              string.Join(", ", assemblies.Select(a => a.GetName().Name)));

            foreach (LifeCycle lifeCycle in Enum.GetValues(typeof(LifeCycle)))
            {
                GetStandardServicesPipeline(builder, assemblies)
                    .PropertiesAutowired()
                    .ConfigureLifecycle(lifeCycle);

                GetKeyedServicesPipeline(builder, assemblies)
                    .PropertiesAutowired()
                    .ConfigureLifecycle(lifeCycle);

                GetOpenGenericServicesPipeline(builder, assemblies)
                    .PropertiesAutowired()
                    .Where(t => t.GetCustomAttribute<RegisterServiceAttribute>() != null
                                && t.GetCustomAttribute<RegisterServiceAttribute>().LifeCycle == lifeCycle)
                    .SetLifecycle(lifeCycle);
            }
        }

        private static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            GetStandardServicesPipeline(ContainerBuilder builder, Assembly[] assemblies)
        {
            return builder.RegisterAssemblyTypes(assemblies)
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
                });
        }

        private static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            GetKeyedServicesPipeline(ContainerBuilder builder, Assembly[] assemblies)
        {
            return builder.RegisterAssemblyTypes(assemblies)
                .Where(t => t.GetCustomAttribute<RegisterHandlerAttribute>() != null)
                .Keyed<ISearchCompleteHandler>(t => t.GetCustomAttribute<RegisterHandlerAttribute>().Key);
        }

        private static IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>
            ConfigureLifecycle(
                this IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle> registrationBuilder,
                LifeCycle lifeCycle)
        {
            return (IRegistrationBuilder<object, ScanningActivatorData, DynamicRegistrationStyle>)registrationBuilder
                .Where(t => t.GetCustomAttribute<RegisterServiceAttribute>() != null
                            && t.GetCustomAttribute<RegisterServiceAttribute>().LifeCycle == lifeCycle)
                .SetLifecycle(lifeCycle);
        }


        private static IRegistrationBuilder<object, OpenGenericScanningActivatorData, DynamicRegistrationStyle>
            GetOpenGenericServicesPipeline(ContainerBuilder builder, Assembly[] assemblies)
        {
            return builder.RegisterAssemblyOpenGenericTypes(assemblies)
                .Where(t => t.GetCustomAttribute<RegisterServiceAttribute>() != null &&
                            t.GetCustomAttribute<RegisterServiceAttribute>().IsOpenGeneric)
                .As(t =>
                {
                    var attribute = t.GetCustomAttribute<RegisterServiceAttribute>();
                    if (attribute.ServiceType != null && attribute.ServiceType.IsGenericTypeDefinition)
                        return new[] { attribute.ServiceType };

                    return t.GetInterfaces().Where(i => i.IsGenericTypeDefinition && i.Name == $"I{t.Name}");
                });
        }

        private static IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>
            SetLifecycle(
                this IRegistrationBuilder<object, ReflectionActivatorData, DynamicRegistrationStyle>
                    registrationBuilder, LifeCycle lifeCycle)
        {
            switch (lifeCycle)
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

            return registrationBuilder;
        }
    }
}