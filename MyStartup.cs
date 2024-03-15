using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using HashTrack.Services;
using Microsoft.Office.Interop.Outlook;

namespace HashTrack
{  
    public static class MyStartup
    {
        public static IContainer Container { get; private set; }

        public static void ConfigureContainer()
        {
            var builder = new ContainerBuilder();
            // Register your services here, for example:
            // builder.RegisterType<YourService>().As<IYourService>();

            // Register Outlook Application session
            builder.Register(c => Globals.ThisAddIn.Application).As<Application>();
            builder.RegisterType<ThisAddIn>().PropertiesAutowired();

            // Register your services here, for example:
            builder.RegisterType<AdvancedSearchCompleteHandler>().As<AdvancedSearchCompleteHandler>().SingleInstance();
            builder.RegisterType<HashTrackSearchWpfControl>().SingleInstance();
            builder.RegisterType<ArtifactSearchService>().As<ArtifactSearchService>();

            Container = builder.Build();
        }
    }

}
