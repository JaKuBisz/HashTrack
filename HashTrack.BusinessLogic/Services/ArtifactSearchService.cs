using System;
using System.Collections.Generic;
using System.Linq;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using Outlook = Microsoft.Office.Interop.Outlook;
using Filter = HashTrack.Core.Constants.DaslFilter.HttpMail;

namespace HashTrack.BusinessLogic.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(IArtifactSearchService))]
    public class ArtifactSearchService : IArtifactSearchService
    {
        private readonly Microsoft.Office.Interop.Outlook.Application _application;
        public ArtifactSearchService(Outlook.Application application) 
        {
            _application = application;

        }

        public void SearchExactMatch(AdvancedSearchQueryOptions searchQuery)//, ArtifactTypes artifactTypes)
        {
            string scope = GetScope(searchQuery.Artefacts);
            string filter = GetFilter(searchQuery.Keyword, searchQuery.From, searchQuery.To, searchQuery.ExactMatch);

            _application.AdvancedSearch(scope, filter, true, searchQuery.Tag);
        }

        private string GetFilter(string keyword, DateTime? from, DateTime? to, bool exactMatch = false)
        {
            var startDate = (from ?? DateTime.MinValue).ToString("yyyy-MM-dd");
            var endDate = (to ?? DateTime.MaxValue).ToString("yyyy-MM-dd");
            
            var wordFilter = exactMatch 
                ? Constants.DaslFilter.ExactMatch(keyword) 
                : Constants.DaslFilter.SubString(keyword);
            //var res = String.Format("urn:schemas:httpmail:textdescription {0} AND urn:schemas:httpmail:date >= '{1}' AND urn:schemas:httpmail:date <= '{2}'", wordFilter, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var filter = $"{Filter.Body} {wordFilter} AND {Filter.Date} >= '{startDate}' AND {Filter.Date} <= '{endDate}'";
            return filter;
            
        }

        private string GetScope(ArtifactTypes artifactTypes)
        {
            var folderList = new List<Outlook.OlDefaultFolders>();

            if (artifactTypes.HasFlag(ArtifactTypes.Email))
            {
                folderList.Add(Outlook.OlDefaultFolders.olFolderInbox);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Appointment))
            {
                folderList.Add(Outlook.OlDefaultFolders.olFolderCalendar);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Contact))
            {
                folderList.Add(Outlook.OlDefaultFolders.olFolderContacts);
            }
            if (artifactTypes.HasFlag(ArtifactTypes.Task))
            {
                folderList.Add(Outlook.OlDefaultFolders.olFolderTasks);
            }

            return string.Join(",", folderList.Select(f => $"'{GetFolderName(f)}'"));

            string GetFolderName(Outlook.OlDefaultFolders defaultFolder)
            {
                return _application.Session.GetDefaultFolder(defaultFolder).Name;
            }
        }
    }
}
