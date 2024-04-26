using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
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

        public void SearchAllItemsForTag(HashTagModel hashTag)
        {
            //TODO: implement missing functionality; handler, indedxing need to be done; so reinddex before etc.
            string scope = GetScope(ArtifactTypes.All);
            string filter = GetAllTagsFilter(hashTag);
            var search = _application.AdvancedSearch(scope, filter, true, Events.CategoryManagerSearch);
        }

        public void SearchItemsByIds(IEnumerable<Guid> artefactIds)
        {
            string scope = GetScope(ArtifactTypes.All);
            string filter = GetIdsFilter(artefactIds);
            throw new NotImplementedException();
            //var search = _application.AdvancedSearch(scope, filter, true, searchQuery.Tag);
        }

        public void SearchExactMatch(AdvancedSearchQueryOptions searchQuery)//, ArtifactTypes artifactTypes)
        {
            string scope = GetScope(searchQuery.Artefacts);
            string filter = GetFilter(searchQuery.Keyword, searchQuery.From, searchQuery.To, searchQuery.ExactMatch);
            var search = _application.AdvancedSearch(scope, filter, true, searchQuery.Tag);
        }

        private string GetFilter(string keyword, DateTime? from, DateTime? to, bool exactMatch = false)
        {
            //TODO: Search by seconds + Add support for multiple keywords
            var startDate = (from ?? DateTime.MinValue).ToString("yyyy-MM-dd"); //
            var endDate = (to ?? DateTime.MaxValue).ToString("yyyy-MM-dd");
            
            var wordFilter = exactMatch 
                ? Constants.DaslFilter.ExactMatch(keyword) 
                : Constants.DaslFilter.SubString(keyword);
            //var res = String.Format("urn:schemas:httpmail:textdescription {0} AND urn:schemas:httpmail:date >= '{1}' AND urn:schemas:httpmail:date <= '{2}'", wordFilter, startDate.ToString("yyyy-MM-dd"), endDate.ToString("yyyy-MM-dd"));
            var filter = $"{Filter.Body} {wordFilter} AND {Filter.Date} >= '{startDate}' AND {Filter.Date} <= '{endDate}'";
            string test = $"{GetCustomPropFilter("TestProp2")} = 'TestSearchPhrase12'"; // or as wordFilter
            return filter;
            
            string GetCustomPropFilter(string propertyName)
            {
                return
                    $"\"http://schemas.microsoft.com/mapi/string/{{00020329-0000-0000-C000-000000000046}}/{propertyName}\"";
            }
        }
        private string GetIdsFilter(IEnumerable<Guid> artefactIds, DateTime? from = null, DateTime? to = null)
        {
            var startDate = (from ?? DateTime.MinValue).ToString("yyyy-MM-dd"); //
            var endDate = (to ?? DateTime.MaxValue).ToString("yyyy-MM-dd");
            var filters = new List<string>();
            foreach (var id in artefactIds)
            {
                var wordFilter = Constants.DaslFilter.ExactMatch(id.ToString());
                filters.Add($"{GetCustomProperty(Constants.CustomProperties.artefactID)} {wordFilter}");
            }
            var filter = string.Join(" OR ", filters);
            return filter;
        }

        private string GetAllTagsFilter(HashTagModel hashTag, DateTime? from = null, DateTime? to = null)
        {
            var startDate = (from ?? DateTime.MinValue).ToString("yyyy-MM-dd"); //
            var endDate = (to ?? DateTime.MaxValue).ToString("yyyy-MM-dd");
            var filters = new List<string>();
            var tags = new List<HashTagModel>(hashTag.TotalMergedHashTags());
            tags.Add(hashTag);
            foreach (var tag in tags)
            {
                var wordFilter = $" = '{tag.Tag}'";
                filters.Add($"{GetCustomProperty(Constants.CustomProperties.Tags)} {wordFilter}");
            }
            var tagsFilter = string.Join(" OR ", filters);
            var filter = $"{tagsFilter} AND {Filter.Date} >= '{startDate}' AND {Filter.Date} <= '{endDate}'";
            return filter;
        }
            
        private string GetCustomProperty(string propertyName)
        {
            return
                $"\"http://schemas.microsoft.com/mapi/string/{{00020329-0000-0000-C000-000000000046}}/{propertyName}\"";
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
