using System;
using System.Timers;
using Autofac;
using HashTrack.Core;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Interfaces.Indexing;
using HashTrack.Properties;
using Microsoft.Office.Tools;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace HashTrack
{
    public partial class ThisAddIn
    {
        private ICache _cache;
        private IEventAggregator _eventAggregator;
        private SidePanelWpfControl _hashTrackSearchWpfControl;
        private IIndexingService _indexingService;
        private Timer _indexingTimer;
        private IPersistenceHashTagService _persistenceHashTagService;
        private IComponentContext _resolver;
        private ISearchCompleteHandlerFactory _searchCompleteHandlerFactory;
        private ISearchService _searchService;
        private CustomTaskPane myCustomTaskPane;

        private void ThisAddIn_Startup(object sender, EventArgs e)
        {
            // Register services
            MyStartup.ConfigureContainer();

            // Resolve services (can not be done in the constructor of the class)
            _resolver = IoC.Startup.ServiceLocator.Resolve<IComponentContext>();
            ResolveServices();

            RegisterEventHandlers();
            StartupServices();

            // Create and initiate the custom task pane
            InicializeUI();
        }

        private void InicializeUI()
        {
            var mainUserControl = new SidePanelPlaceholder(_hashTrackSearchWpfControl);
            myCustomTaskPane = CustomTaskPanes.Add(mainUserControl, "HashTrack - Hash search");
            myCustomTaskPane.Visible = true;
            myCustomTaskPane.Width = 350;
        }

        private void RegisterEventHandlers()
        {
            //TODO: Use Async use eventHandler from Outlook Office object
            //TODO: Use IEventAggregator instead of directly this and add Task.Run to make it asynchronous as these event are only synchronous
            Application.AdvancedSearchComplete += _searchCompleteHandlerFactory.HandleSearchCompleted;
            ((Outlook.ApplicationEvents_11_Event)Application).Quit += ShutdownServices;
        }

        private void ResolveServices()
        {
            _searchCompleteHandlerFactory = _resolver.Resolve<ISearchCompleteHandlerFactory>();
            _searchService = _resolver.Resolve<ISearchService>();
            _eventAggregator = _resolver.Resolve<IEventAggregator>();
            _cache = _resolver.Resolve<ICache>();
            _indexingService = _resolver.Resolve<IIndexingService>();
            _hashTrackSearchWpfControl = _resolver.Resolve<SidePanelWpfControl>();
            _persistenceHashTagService = _resolver.Resolve<IPersistenceHashTagService>();
        }

        private void StartupServices()
        {
            RunIndexing();
            CacheDataFromStorage();
        }

        private void CacheDataFromStorage()
        {
            var hashTags = _persistenceHashTagService.GetAllHashTags();
            _cache.AddHashTags(hashTags);
            _eventAggregator.FireEvent(Events.HashTagsUpdated);
        }

        private void RunIndexing(object sender = null, ElapsedEventArgs e = null)
        {
            var lastIndexingDate = Settings.Default.LastIndexingDateTime;
            _searchService.PerformIndexing(lastIndexingDate);
            Settings.Default.LastIndexingDateTime = DateTime.Now;
            Settings.Default.Save();
        }

        private void SetIndexingTimer()
        {
            _indexingTimer = new Timer(300000); // 5 minutes;
            _indexingTimer.Elapsed += RunIndexing;
            _indexingTimer.AutoReset = true;
            _indexingTimer.Enabled = true;
        }

        private void ShutdownServices()
        {
            _indexingTimer.Dispose();
            Application.AdvancedSearchComplete -= _searchCompleteHandlerFactory.HandleSearchCompleted;
            ((Outlook.ApplicationEvents_11_Event)Application).Quit -= ShutdownServices;
        }

        private void ThisAddIn_Shutdown(object sender, EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
            ShutdownServices();
        }

        #region VSTO generated code

        /// <summary>
        ///     Required method for Designer support - do not modify
        ///     the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            Startup += ThisAddIn_Startup;
            Shutdown += ThisAddIn_Shutdown;
        }

        #endregion
    }
}