using System;
using System.Collections.Generic;
using Autofac;
using HashTrack.BusinessLogic.Services;
using HashTrack.Core;
using HashTrack.IoC;
using HashTrack.Persistence.Contexts;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack
{
    public static class MyStartup
    {
        public static void ConfigureContainer()
        {
            ForceLoadAssemblies();
            Startup.ConfigureContainer(builder =>
            {
                builder.Register(c => Globals.ThisAddIn.Application).As<Application>().InstancePerLifetimeScope();
                builder.RegisterType<ThisAddIn>().PropertiesAutowired();
                //builder.RegisterType<SidePanelWpfControl>().SingleInstance();
            });
        }

        private static void ForceLoadAssemblies()
        {
            // Force load assemblies
            var types = new List<Type>();
            types.Add(typeof(HashTrackDbContext)); // HashTrack.Persistence
            types.Add(typeof(SearchService)); // HashTrack.BusinessLogic
            types.Add(typeof(Constants)); // HashTrack.Core
            types.Add(typeof(ServiceLocator)); // HashTrack.IoC
        }
    }
}