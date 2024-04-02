using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Helpers;
using Microsoft.Office.Interop.Outlook;
using System;
using System.Collections.Generic;
using System.Linq;
using Filter = HashTrack.Helpers.Constants.DaslFilter.HttpMail;

namespace HashTrack.Services
{
    [RegisterService(typeof(ArtifactSearchService), LifeCycle.Singleton)]
    public class ArtifactSearchService
    {
        private readonly Application _application;
        public ArtifactSearchService(Application application) 
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
