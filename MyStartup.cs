using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using Autofac.Builder;
using HashTrack.Attributes;
using HashTrack.Enums;
using HashTrack.Helpers;
using HashTrack.Interfaces;
using HashTrack.Services;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack
{  
    public static class MyStartup
    {
        public static IContainer Container { get; private set; }
        public static ServiceLocator ServiceLocator { get; private set; }

        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            // Register your services here, for example:
            // builder.RegisterType<YourService>().As<IYourService>();

            // Register Outlook Application session
            builder.Register(c => Globals.ThisAddIn.Application).As<Application>();
            builder.RegisterType<ThisAddIn>().PropertiesAutowired();

            RegisterServices(builder);
            // Register your services here, for example:
            //builder.RegisterType<AdvancedSearchCompleteHandler>().As<AdvancedSearchCompleteHandler>().SingleInstance();
            //builder.RegisterType<HashTrackSearchWpfControl>().SingleInstance();
            builder.RegisterType<ArtifactSearchService>().As<ArtifactSearchService>();

            Container = builder.Build();

            var scope = Container.BeginLifetimeScope();
            ServiceLocator = new ServiceLocator(scope);
        }

        private static void RegisterServices(ContainerBuilder builder)
        {
            var executingAssembly = Assembly.GetExecutingAssembly();
            // Start with the executing assembly
            var assembliesToScan = new List<Assembly> { executingAssembly };

            // Add referenced assemblies
            assembliesToScan.AddRange(GetReferencedAssemblies(executingAssembly));

            foreach (var assembly in assembliesToScan)
            {
                RegisterDecoratedServicesForAssembly(builder, assembly);
            }
        }

        private static void RegisterDecoratedServicesForAssembly(ContainerBuilder builder, Assembly assembly)
        {
            // Get all types from the current assembly
            var types = assembly.GetTypes();

            foreach (var type in types)
            {
                // Get the RegisterServiceAttribute on this type, if it exists
                var registerServiceAttribute = type.GetCustomAttribute<RegisterServiceAttribute>();

                if (registerServiceAttribute != null)
                {               
                    IRegistrationBuilder<object, ConcreteReflectionActivatorData, SingleRegistrationStyle> registrationBuilder;

                    var handlerAttribute = type.GetCustomAttribute<RegisterHandlerAttribute>();
                    if (handlerAttribute != null)
                    {
                        // Register handler with Autofac
                        registrationBuilder = builder.RegisterType(type).Keyed(handlerAttribute.Tag, handlerAttribute.ServiceType);
                    }
                    else
                    {
                        // Register service with Autofac
                        registrationBuilder = builder.RegisterType(type).As(registerServiceAttribute.ServiceType);
                    }

                    // Set the lifecycle
                    switch (registerServiceAttribute.LifeCycle)
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
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
                }
            }
        }

        // Helper method to get referenced assemblies
        private static IEnumerable<Assembly> GetReferencedAssemblies(Assembly assembly)
        {
            var referencedAssembliesNames = assembly.GetReferencedAssemblies();
            foreach (var assemblyName in referencedAssembliesNames)
            {
                // Load and yield each referenced assembly
                yield return Assembly.Load(assemblyName);
            }
        }
    }

}
