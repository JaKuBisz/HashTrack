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
    [RegisterService(LifeCycle.Singleton, typeof(ISearchService))]
    public class SearchService : ISearchService
    {
        private readonly Outlook.Application _application;

        public SearchService(Outlook.Application application)
        {
            _application = application;
        }

        public void SearchTags(SearchTagsQueryOptions searchTagsQuery)
        {
            var scope = GetScope(searchTagsQuery.Artefacts);
            var filter = GetFilter(searchTagsQuery);
            _application.AdvancedSearch(scope, filter, true, searchTagsQuery.EventTag);
        }

        public void PerformIndexing(DateTime? from = null, DateTime? to = null)
        {
            // Index all artifacts in last 30 days
            from = from ?? DateTime.Now.AddDays(-30);
            to = to ?? DateTime.Now;
            var query = new SearchTagsQueryOptions
            {
                Tag = "#",
                Artefacts = ArtifactTypes.All,
                From = from,
                To = to,
                EventTag = Events.IndexingSearchCompleted,
                ExactMatch = false,
                UseCustomProperty = false
            };

            SearchTags(query);
        }

        public void SearchAllItemsForTag(HashTagModel hashTag)
        {
            //TODO: implement missing functionality; handler, indedxing need to be done; so reinddex before etc.
            var scope = GetScope(ArtifactTypes.All);
            var filter = GetAllTagsFilter(hashTag);
            var search = _application.AdvancedSearch(scope, filter, true, Events.CategoryManagerSearch);
        }


        private string GetFilter(SearchTagsQueryOptions searchTagsQuery)
        {
            var tags = searchTagsQuery.Tags ?? new List<string> { searchTagsQuery.Tag };
            var filters = new List<string>();
            var wordFilter = string.Empty;
            var filterProperty = string.Empty;

            var startDate = (searchTagsQuery.From ?? DateTime.MinValue).ToString("yyyy-MM-dd"); //
            var endDate = (searchTagsQuery.To ?? DateTime.Now).AddDays(1).ToString("yyyy-MM-dd");

            foreach (var tag in tags)
            {
                if (searchTagsQuery.UseCustomProperty && tag.Contains("#"))
                {
                    wordFilter = Constants.DaslFilter.Equals(tag);
                    filterProperty = GetCustomProperty(Constants.CustomProperties.Tags);
                }
                else
                {
                    wordFilter = searchTagsQuery.ExactMatch
                        ? Constants.DaslFilter.ExactMatch(tag)
                        : Constants.DaslFilter.SubString(tag);
                    filterProperty = Filter.Body;
                }

                filters.Add($"{filterProperty} {wordFilter}");
            }

            var tagsFilter = string.Join(" OR ", filters);

            var filter = $"({tagsFilter}) AND {Filter.Date} >= '{startDate}' AND {Filter.Date} <= '{endDate}'";
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
                var wordFilter = $" = '{tag.Id}'";
                filters.Add($"{GetCustomProperty(Constants.CustomProperties.Tags)} {wordFilter}");
            }

            var tagsFilter = string.Join(" OR ", filters);
            var filter = $"({tagsFilter}) AND {Filter.Date} >= '{startDate}' AND {Filter.Date} <= '{endDate}'";
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

            if (artifactTypes.HasFlag(ArtifactTypes.Email)) folderList.Add(Outlook.OlDefaultFolders.olFolderInbox);
            if (artifactTypes.HasFlag(ArtifactTypes.Appointment))
                folderList.Add(Outlook.OlDefaultFolders.olFolderCalendar);
            if (artifactTypes.HasFlag(ArtifactTypes.Contact)) folderList.Add(Outlook.OlDefaultFolders.olFolderContacts);
            if (artifactTypes.HasFlag(ArtifactTypes.Task)) folderList.Add(Outlook.OlDefaultFolders.olFolderTasks);

            return string.Join(",", folderList.Select(f => $"'{GetFolderName(f)}'"));

            string GetFolderName(Outlook.OlDefaultFolders defaultFolder)
            {
                return _application.Session.GetDefaultFolder(defaultFolder).Name;
            }
        }
    }
}