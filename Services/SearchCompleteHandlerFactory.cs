using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autofac;
using HashTrack.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    //TODO: Reowrk this as handler caller that will resolve handler for specific tag
    [RegisterService(typeof(SearchCompleteHandlerFactory), LifeCycle.Singleton)]
    public class SearchCompleteHandlerFactory
    {
        private readonly IComponentContext _context;
        public SearchCompleteHandlerFactory(IComponentContext context)
        {
            _context = context;
        }
        
        public void HandleSearchCompleted(Outlook.Search SearchObject)
        {
            var handler = _context.ResolveKeyed<ISearchCompleteHandler>(SearchObject.Tag);
                handler.HandleSearchComplete(SearchObject);
        }
    }
}
