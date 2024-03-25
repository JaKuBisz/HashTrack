using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    [RegisterService(typeof(ArtifactSearchService), LifeCycle.Singleton)]
    internal class ArtifactSearchService
    {
        private readonly Application _application;
        public ArtifactSearchService(Application application) 
        {
            _application = application;

        }

        public void Search(AdvancedSearchQueryDto searchQuery)//, ArtifactTypes artifactTypes)
        {
            string scope = GetScope(searchQuery.Artefacts);
            string filter = GetFilter(searchQuery.Keyword, searchQuery.From, searchQuery.To);

            var search = _application.AdvancedSearch(scope, filter, true, Constants.DefaultSearchTag);


            /*
            Outlook.MAPIFolder inboxFolder = _application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox);
            var inboxDefault = inboxFolder.Name;

            string scope = "'" + inboxFolder.FolderPath + "'";
            Outlook.Search search = Globals.ThisAddIn.Application.AdvancedSearch($"'{inboxDefault}','Contacts','Calendar', 'Tasks'", filter, true, "Test");
*/
         
        }

        private string GetFilter(string keyword, DateTime? from, DateTime? to)
        {
            DateTime startDate = from ?? DateTime.MinValue;
            DateTime endDate = to ?? DateTime.MaxValue;
            //DateTime expirationDate = DateTime.Now.AddDays(30);
            //string expiresFilter = String.Format("urn:schemas:mailheader:expires>'{0}' AND urn:schemas:mailheader:expires<'{0}'", DateTime.Now, expirationDate);
            return String.Format("urn:schemas:httpmail:textdescription ci_phrasematch '{0}' AND urn:schemas:httpmail:date >= '{1}' AND urn:schemas:httpmail:date <= '{2}'", keyword, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));

            //return String.Format("urn:schemas:httpmail:textdescription ci_phrasematch '{0}'", keyword);
        }

        private string GetScope(ArtifactTypes artifactTypes)
        {
            var folderList = new List<OlDefaultFolders>();

            if (artifactTypes.HasFlag(ArtifactTypes.Email))
            {
                folderList.Add(OlDefaultFolders.olFolderInbox);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Appointment))
            {
                folderList.Add(OlDefaultFolders.olFolderCalendar);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Contact))
            {
                folderList.Add(OlDefaultFolders.olFolderContacts);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Task))
            {
                folderList.Add(OlDefaultFolders.olFolderTasks);
            }

            return string.Join(",", folderList.Select(f => $"'{GetFolderName(f)}'"));

            string GetFolderName(OlDefaultFolders defaultFolder)
            {
                return _application.Session.GetDefaultFolder(defaultFolder).Name;
            }
        }
    }
}
