using Autofac;
using HashTrack.Core;
using HashTrack.Core.Interfaces.Handlers;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Interfaces;
using HashTrack.Interfaces.Indexing;

namespace HashTrack
{
    public partial class ThisAddIn
    {
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        private ISearchCompleteHandlerFactory _searchCompleteHandlerFactory;
        private IArtifactSearchService _artifactSearchService;
        private SidePanelWpfControl _hashTrackSearchWpfControl;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Register services
            MyStartup.ConfigureContainer();

            // Resolve services (can not be done in the constructor of the class)

            var resolver = IoC.Startup.ServiceLocator.Resolve<IComponentContext>();
            var service = resolver.ResolveKeyed<ISearchCompleteHandler>(Constants.DefaultSearchTag);
            _searchCompleteHandlerFactory = resolver.Resolve<ISearchCompleteHandlerFactory>();
            _artifactSearchService = resolver.Resolve<IArtifactSearchService>();
            var indexingService = resolver.Resolve<IIndexingService>();

            _hashTrackSearchWpfControl = resolver.Resolve<SidePanelWpfControl>();

            // Register event handlers
            Application.AdvancedSearchComplete += _searchCompleteHandlerFactory.HandleSearchCompleted;
            _hashTrackSearchWpfControl.SearchInitiated += _artifactSearchService.SearchExactMatch;
            //var _hashTrackSearchWpfControl = new SidePanelWpfControl();
            indexingService.IndexAllArtifacts();

            // Create and initiate the custom task pane
            var myUserControl1 = new SidePanelPlaceholder(_hashTrackSearchWpfControl);
            myCustomTaskPane = this.CustomTaskPanes.Add(myUserControl1, "HashTrack - Hash search");
            myCustomTaskPane.Visible = true;
            myCustomTaskPane.Width = 350;
        }
        private void ThisAddIn_Shutdown(object sender, System.EventArgs e)
        {
            // Note: Outlook no longer raises this event. If you have code that 
            //    must run when Outlook shuts down, see https://go.microsoft.com/fwlink/?LinkId=506785
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
