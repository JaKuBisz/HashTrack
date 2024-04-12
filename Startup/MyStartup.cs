using System;
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
                builder.Register(c => Globals.ThisAddIn.Application).As<Microsoft.Office.Interop.Outlook.Application>().InstancePerLifetimeScope();
                builder.RegisterType<ThisAddIn>().PropertiesAutowired();
                //builder.RegisterType<SidePanelWpfControl>().SingleInstance();
            });
        }

        private static void ForceLoadAssemblies()
        {
            // Force load assemblies
            var _ = typeof(HashTrackDbContext); // HashTrack.Persistence
            _ = typeof(ArtifactSearchService); // HashTrack.BusinessLogic
            _ = typeof(Constants); // HashTrack.Core
            _ = typeof(ServiceLocator); // HashTrack.IoC
        }
    }
}
