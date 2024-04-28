using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using HashTrack.Enums;
using HashTrack.Extensions;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(SearchViewModel))]
    public class SearchViewModel : BaseViewModel
    {
        //Services
        private readonly ICache _cache;
        private readonly IPersistenceHashTagService _hashTagService;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISearchService _searchService;
        private readonly IMessageService _messageService;
        //Fields
        private SearchFilters _searchFilters = new SearchFilters();
        private ObservableCollection<ArtefactModel> _searchResults = new ObservableCollection<ArtefactModel>();
        private OrderBySearch _selectedOrderBy = OrderBySearch.Date;
        //Commands
        public ICommand SearchCommand { get; private set; }
        public ICommand OpenArtefact { get; private set; }
        
        public SearchViewModel(
            IEventAggregator eventAggregator,
            IPersistenceHashTagService hashTagService,
            ICache cache,
            ISearchService searchService,
            IMessageService messageService)
        {
            _eventAggregator = eventAggregator;
            _hashTagService = hashTagService;
            _cache = cache;
            _searchService = searchService;
            _messageService = messageService;
            
            InitializeCommands();
            eventAggregator.Subscribe(Events.DefaultSearchProcessed, UpdateSearchResults);
            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
        }

        private void InitializeCommands()
        {
            SearchCommand = new RelayCommand(ExecuteSearch);
            OpenArtefact = new RelayCommand<ArtefactModel>(OpenArtifact);
        }

        private void UpdateSearchResults()
        {
            var artefacts = _cache.GetArtefacts();
            SetSearchResults(artefacts);
        }

        private void ExecuteTabChange(object obj)
        {
            if (!(obj is ChangeTabEvent evt) || evt.TagModel is null || evt.Target != ChangeTabEventTarget.SearchTab)
            {
                return;
            }
            
            SearchFilters.SearchText = evt.TagModel.Id;
            ExecuteSearch();
        }

        #region Command handlers

        private void ExecuteSearch()
        {
            try
            {
                var searchQuery = SearchFilters.GetSearchQuery();
                if (!searchQuery.Verify())
                {
                    _messageService.ShowMessage("Search query is incorrect", "Search query validation failed", MessageType.Warning);
                    return;
                }

                if (_hashTagService.GetHashTag(searchQuery.Tag) is HashTagModel hashTag
                    && hashTag.HasMergedTags)
                {
                    var mergedTags = hashTag.MergedHashTags.Select(x => x.Id).ToHashSet();
                    mergedTags.Add(hashTag.Id);
                    searchQuery.Tag = null;
                    searchQuery.Tags = mergedTags;
                    if (!searchQuery.Verify())
                    {
                        _messageService.ShowMessage("Search query is incorrect", "Search query validation failed", MessageType.Warning);
                        return;
                    }
                }

                _searchService.SearchTags(searchQuery);
            }
            catch (System.Exception ex)
            {
                _messageService.ShowMessage(ex);
                throw;
            }
        }
        
        private void OpenArtifact(ArtefactModel content)
        {
            switch (content.OriginalItem)
            {
                case Outlook.MailItem mailItem:
                    mailItem.Display(true);
                    break;
                case Outlook.AppointmentItem appointmentItem:
                    appointmentItem.Display(true);
                    break;
                case Outlook.ContactItem contactItem:
                    contactItem.Display(true);
                    break;
                case Outlook.TaskItem taskItem:
                    taskItem.Display(true);
                    break;
            }
        }

#endregion
#region Helpers
        private void SetSearchResults(List<ArtefactModel> searchResults)
        {
            var orderedResults = Order(searchResults);
            _searchResults = new ObservableCollection<ArtefactModel>(orderedResults);
            OnPropertyChanged(nameof(SearchResults));
        }
        
        private void ReOrder()
        {
            var orderedResults = Order(SearchResults);
            SearchResults = new ObservableCollection<ArtefactModel>(orderedResults);
            OnPropertyChanged(nameof(SearchResults));
        }

        private IEnumerable<ArtefactModel> Order(IEnumerable<ArtefactModel> searchResults)
        {
            IEnumerable<ArtefactModel> orderedResults;
            switch (OrderBy)
            {
                case OrderBySearch.Date:
                    orderedResults = searchResults.OrderByDescending(x => x.Date);
                    break;
                case OrderBySearch.Sender:
                    orderedResults = searchResults.OrderBy(x => x.Sender);
                    break;
                case OrderBySearch.Title:
                    orderedResults = searchResults.OrderBy(x => x.Title);
                    break;
                default:
                    orderedResults = searchResults;
                    break;
            }
            
            return orderedResults;
        }
#endregion
#region Properties
        public List<OrderBySearch> OrderByOptions { get; } =
            Enum.GetValues(typeof(OrderBySearch)).Cast<OrderBySearch>().ToList();

        public OrderBySearch OrderBy
        {
            get => _selectedOrderBy;
            set
            {
                SetField(ref _selectedOrderBy, value);
                ReOrder();
            }
        }

        public SearchFilters SearchFilters
        {
            get => _searchFilters;
            set => SetField(ref _searchFilters, value);
        }

        public ObservableCollection<ArtefactModel> SearchResults
        {
            get => _searchResults;
            set => SetField(ref _searchResults, value);
        }
#endregion
    }

    public class SearchFilters : BaseViewModel
    {
        private string _searchText;
        public bool IsEmailChecked { get; set; } = true;
        public bool IsAppointmentsChecked { get; set; } = true;
        public bool IsTasksChecked { get; set; } = true;
        public bool IsContactsChecked { get; set; } = true;
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }

        public SearchTagsQueryOptions GetSearchQuery()
        {
            return new SearchTagsQueryOptions
            {
                Tag = SearchText,
                Artefacts = EvaluateArtefactsSelection(),
                From = FromDate,
                To = ToDate,
                EventTag = Events.DefaultSearchCompleted,
                ExactMatch = true,
            };
        }
        
        public string SearchText
        {
            get => _searchText;
            set => SetField(ref _searchText, value);
        }

        public ArtifactTypes EvaluateArtefactsSelection()
        {
            var artefacts = ArtifactTypes.None;

            if (IsEmailChecked)
            {
                artefacts |= ArtifactTypes.Email;
            }
            if (IsAppointmentsChecked)
            {
                artefacts |= ArtifactTypes.Appointment;
            }
            if (IsContactsChecked)
            {
                artefacts |= ArtifactTypes.Contact;
            }
            if (IsTasksChecked)
            {
                artefacts |= ArtifactTypes.Task;
            }
            /*
            if (artefacts == ArtifactTypes.None)
            {
                throw new SearchQueryException("Please select at least one artefact type to search for.");
            }*/
            return artefacts;
        }
    }
}