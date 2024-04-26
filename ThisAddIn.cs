using Autofac;
using HashTrack.Core;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using HashTrack.Interfaces;
using HashTrack.Interfaces.Indexing;
using HashTrack.Persistence.Entities;
using HashTrack.Persistence.Interfaces;
using Microsoft.Office.Tools.Ribbon;
using System;
using System.Collections.Generic;
using System.Timers;
using Outlook = Microsoft.Office.Interop.Outlook;


namespace HashTrack
{
    public partial class ThisAddIn
    {
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        private ISearchCompleteHandlerFactory _searchCompleteHandlerFactory;
        private IArtifactSearchService _artifactSearchService;
        private SidePanelWpfControl _hashTrackSearchWpfControl;
        private IIndexingService _indexingService;
        private IComponentContext _resolver;
        private Timer _indexingTimer;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Register services
            MyStartup.ConfigureContainer();

            // Resolve services (can not be done in the constructor of the class)
            _resolver = IoC.Startup.ServiceLocator.Resolve<IComponentContext>();
            ResolveServices();
            
            // Register event handlers
            RegisterEventHandlers();
            RunIndexing();

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
        }
        private void ResolveServices()
        {
            _searchCompleteHandlerFactory = _resolver.Resolve<ISearchCompleteHandlerFactory>();
            _artifactSearchService = _resolver.Resolve<IArtifactSearchService>();
            _indexingService = _resolver.Resolve<IIndexingService>();
            _hashTrackSearchWpfControl = _resolver.Resolve<SidePanelWpfControl>();
        }

        private void RunIndexing(object sender = null, ElapsedEventArgs e = null)
        {
            var lastIndexingDate = Properties.Settings.Default.LastIndexingDateTime;
            _indexingService.IndexAllArtifacts(lastIndexingDate);
            Properties.Settings.Default.LastIndexingDateTime = DateTime.Now;
        }
        
        private void SetIndexingTimer()
        {
            _indexingTimer = new Timer(300000); // 5 minutes;
            _indexingTimer.Elapsed += RunIndexing;
            _indexingTimer.AutoReset = true;
            _indexingTimer.Enabled = true;
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
            _indexingTimer.Dispose();
            Application.AdvancedSearchComplete -= _searchCompleteHandlerFactory.HandleSearchCompleted;
        }

        #region VSTO generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InternalStartup()
        {
            this.Startup += new System.EventHandler(ThisAddIn_Startup);
            this.Shutdown += new System.EventHandler(ThisAddIn_Shutdown);
        }
        
        #endregion
    }
}
