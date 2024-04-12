using Autofac;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Interfaces;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services
{
    //TODO: Reowrk this as handler caller that will resolve handler for specific tag
    [RegisterService(LifeCycle.Transient, typeof(ISearchCompleteHandlerFactory))]
    public class SearchCompleteHandlerFactory : ISearchCompleteHandlerFactory
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
