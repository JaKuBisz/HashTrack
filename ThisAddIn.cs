using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Linq;
using Outlook = Microsoft.Office.Interop.Outlook;
using Office = Microsoft.Office.Core;
using System.Windows.Forms;
using System.Diagnostics;
using HashTrack.Services;
using Autofac;
using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;

namespace HashTrack
{
    public partial class ThisAddIn
    {
        private Microsoft.Office.Tools.CustomTaskPane myCustomTaskPane;
        private SearchCompleteHandlerFactory _searchCompleteHandlerFactory;
        private ArtifactSearchService _artifactSearchService;
        private HashTrackSearchWpfControl _hashTrackSearchWpfControl;

        private void ThisAddIn_Startup(object sender, System.EventArgs e)
        {
            // Register services
            MyStartup.ConfigureContainer();

            // Resolve services (can not be done in the constructor of the class)
            _searchCompleteHandlerFactory = MyStartup.ServiceLocator.Resolve<SearchCompleteHandlerFactory>();
            _artifactSearchService = MyStartup.ServiceLocator.Resolve<ArtifactSearchService>();
            _hashTrackSearchWpfControl = MyStartup.ServiceLocator.Resolve<HashTrackSearchWpfControl>();
            var indexingService = MyStartup.ServiceLocator.Resolve<IndexingService>();

            // Register event handlers
            Application.AdvancedSearchComplete += _searchCompleteHandlerFactory.HandleSearchCompleted;
            _hashTrackSearchWpfControl.SearchInitiated += _artifactSearchService.SearchExactMatch;
            //var _hashTrackSearchWpfControl = new HashTrackSearchWpfControl();
            indexingService.IndexAllArtifacts();

            // Create and initiate the custom task pane
            var myUserControl1 = new UserControl1(_hashTrackSearchWpfControl);
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
