using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
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
using HashTrack.Properties;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagOverviewViewModel))]
    public class HashTagOverviewViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISearchService _searchService;
        private DateTime? _fromDate;

        private ObservableCollection<HashTagModel> _indexingHashtags;
        private int _maxOccurrences;
        private int _minOccurrences;
        private string _searchBar;
        private OrderByHashTagsOverview _selectedOrderBy = OrderByHashTagsOverview.OccurrencesDesc;
        private DateTime? _toDate;


        public HashTagOverviewViewModel(IEventAggregator eventAggregator, ICache cache, ISearchService searchService)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>();
            _eventAggregator = eventAggregator;
            _cache = cache;
            _searchService = searchService;
            //TODO: Use Async use eventHandler from Outlook Office object
            eventAggregator.Subscribe(Events.HashTagsUpdated, UpdateIndexingResults);
            InitializeCommands();
        }

        private void InitializeCommands()
        {
            StartIndexingCommand = new RelayCommand(StartIndexing);
            HashTagItemDoubleClick = new RelayCommand<object>(list_Hashtags_MouseDoubleClick);
            OpenTagDetail = new RelayCommand<object>(ExecuteOpenTagDetail);
            OpenSearchResultsCommand = new RelayCommand<object>(ExecuteOpenSearchResults);
            MergeTags = new RelayCommand<object>(ExecuteMergeTags);
        }

        private ObservableCollection<HashTagModel> FilterIndexingResults(
            ObservableCollection<HashTagModel> indexingHashtags)
        {
            if (string.IsNullOrWhiteSpace(SearchBar) && MinOccurrences <= 0 && MaxOccurrences <= 0)
                return indexingHashtags;

            var result = indexingHashtags.Where(x =>
                (string.IsNullOrWhiteSpace(SearchBar) || x.Id.Contains(SearchBar))
                && x.NumOfOccurrences >= MinOccurrences
                && (MaxOccurrences <= 0 || x.NumOfOccurrences <= MaxOccurrences));

            return new ObservableCollection<HashTagModel>(result);
        }

        private void ExecuteOpenTagDetail(object obj)
        {
            if (obj is HashTagModel model)
            {
                var evt = new ChangeTabEvent(ChangeTabEventTarget.TagDetailsTab, model);
                _eventAggregator.FireEvent(ChangeTabEvent.Tag, evt);
            }
        }

        private void ExecuteOpenSearchResults(object obj)
        {
            if (obj is HashTagModel model)
            {
                var evt = new ChangeTabEvent(ChangeTabEventTarget.SearchTab, model);
                _eventAggregator.FireEvent(ChangeTabEvent.Tag, evt);
            }
        }

        private void ExecuteMergeTags(object obj)
        {
            if (obj is HashTagModel model)
            {
                var evt = new ChangeTabEvent(ChangeTabEventTarget.TagDetailsTab, model);
                _eventAggregator.FireEvent(ChangeTabEvent.Tag, evt);
            }
        }

        private void SetIndexingResult(List<HashTagModel> result)
        {
            IndexingHashtags = new ObservableCollection<HashTagModel>(Order(result));
        }

        private void UpdateIndexingResults()
        {
            var hashTags = _cache.GetHashTags();
            SetIndexingResult(hashTags.ToList());
        }

        //TODO: FIx this - currently will delete cached need to implement a caching service
        private void IndexingFilrerByNumOfOccurencesMin(int minOccurences)
        {
            _indexingHashtags =
                new ObservableCollection<HashTagModel>(
                    _indexingHashtags.Where(x => x.NumOfOccurrences >= minOccurences));
        }

        private void IndexingFilrerByNumOfOccurencesMax(int maxOccurences)
        {
            _indexingHashtags =
                new ObservableCollection<HashTagModel>(
                    _indexingHashtags.Where(x => x.NumOfOccurrences <= maxOccurences));
        }

        private void ReOrder()
        {
            var orderedResults = Order(_indexingHashtags);
            IndexingHashtags = new ObservableCollection<HashTagModel>(orderedResults);
        }

        private IEnumerable<HashTagModel> Order(IEnumerable<HashTagModel> items)
        {
            IEnumerable<HashTagModel> orderedItems;
            switch (OrderBy)
            {
                /*case OrderByHashTagsOverview.DateDesc:
                    orderedItems = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.()));
                    break;*/
                case OrderByHashTagsOverview.OccurrencesDesc:
                    orderedItems = items.OrderByDescending(x => x.TotalNumOfOccurences());
                    break;
                case OrderByHashTagsOverview.OccurrencesAsc:
                    orderedItems = items.OrderBy(x => x.TotalNumOfOccurences());
                    break;
                default:
                    orderedItems = items;
                    break;
            }

            return orderedItems;
        }

        #region Properties

        public List<OrderByHashTagsOverview> OrderByOptions { get; } =
            Enum.GetValues(typeof(OrderByHashTagsOverview)).Cast<OrderByHashTagsOverview>().ToList();

        public OrderByHashTagsOverview OrderBy
        {
            get => _selectedOrderBy;
            set
            {
                SetField(ref _selectedOrderBy, value);
                ReOrder();
            }
        }

        public ObservableCollection<HashTagModel> IndexingHashtags
        {
            get => FilterIndexingResults(_indexingHashtags);
            set => SetField(ref _indexingHashtags, value);
        }

        public DateTime? FromDate
        {
            get => _fromDate;
            set => SetField(ref _fromDate, value);
        }

        public DateTime? ToDate
        {
            get => _toDate;
            set => SetField(ref _toDate, value);
        }

        public int MinOccurrences
        {
            get => _minOccurrences;
            set
            {
                SetField(ref _minOccurrences, value);
                OnPropertyChanged(nameof(IndexingHashtags));
            }
        }

        public int MaxOccurrences
        {
            get => _maxOccurrences;
            set
            {
                SetField(ref _maxOccurrences, value);
                OnPropertyChanged(nameof(IndexingHashtags));
            }
        }

        public string SearchBar
        {
            get => _searchBar;
            set
            {
                SetField(ref _searchBar, value);
                OnPropertyChanged(nameof(IndexingHashtags));
            }
        }

        #endregion

        #region Commands

        public ICommand StartIndexingCommand { get; private set; }
        public ICommand HashTagItemDoubleClick { get; private set; }
        public ICommand OpenTagDetail { get; private set; }
        public ICommand OpenSearchResultsCommand { get; private set; }
        public ICommand MergeTags { get; private set; }

        #endregion


        #region Events

        private void list_Hashtags_MouseDoubleClick(object obj)
        {
            if (obj is HashTagModel model)
            {
                var evt = new ChangeTabEvent(ChangeTabEventTarget.SearchTab, model);
                _eventAggregator.FireEvent(ChangeTabEvent.Tag, evt);
            }
        }

        private void StartIndexing()
        {
            _searchService.PerformIndexing(_fromDate, _toDate);
            Settings.Default.LastIndexingDateTime = DateTime.Now;
            Settings.Default.Save();
        }
        #endregion
    }
}