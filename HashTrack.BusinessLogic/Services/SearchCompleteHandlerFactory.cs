using System;
using Autofac;
using Autofac.Core.Registration;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
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
        private readonly IEventAggregator _eventAggregator;

        public SearchCompleteHandlerFactory(IComponentContext context, IEventAggregator eventAggregator)
        {
            _context = context;
            _eventAggregator = eventAggregator;
        }

        public void HandleSearchCompleted(Outlook.Search searchObject)
        {
            try
            {
                var handler = _context.ResolveKeyed<ISearchCompleteHandler>(searchObject.Tag);
                handler.HandleSearchComplete(searchObject);
            }
            catch (ComponentNotRegisteredException e)
            {
                _eventAggregator.FireEvent(searchObject.Tag, searchObject);
            }
        }
    }
}
