using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Clustering.DTOs;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;
using HashTrack.Enums;
using HashTrack.Extensions;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagOverviewViewModel))]
    public class HashTagOverviewViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private readonly IEventAggregator _eventAggregator;
        
        private ObservableCollection<HashTagModel> _indexingHashtags;
        private DateTime? _fromDate;
        private DateTime? _toDate;
        private int _minOccurrences;
        private int _maxOccurrences;
        private string _searchBar;
        private OrderByHashTagsOverview _selectedOrderBy = OrderByHashTagsOverview.OccurrencesDesc;

        
        public HashTagOverviewViewModel(IEventAggregator eventAggregator, ICache cache)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>();
            _eventAggregator = eventAggregator;
            _cache = cache;
            //TODO: Use Async use eventHandler from Outlook Office object
            eventAggregator.Subscribe(Events.IndexingSearchProcessed, UpdateIndexingResults);
            InitializeCommands();
            _indexingHashtags = new ObservableCollection<HashTagModel>()
            {
                new HashTagModel() { Tag = "#test", NumOfOccurrences = 100 },
                new HashTagModel() { Tag = "#test2", NumOfOccurrences = 200 },
                new HashTagModel() { Tag = "#test3", NumOfOccurrences = 300 },
                new HashTagModel() { Tag = "#test4", NumOfOccurrences = 400 },
                new HashTagModel() { Tag = "#test5", NumOfOccurrences = 500 },
                new HashTagModel() { Tag = "#test6", NumOfOccurrences = 600 },
            };
        }

        private void InitializeCommands()
        {
            StartIndexingCommand = new RelayCommand(StartIndexing);
            OrderByChangedCommand = new RelayCommand(OrderByChanged);
            
            HashTagItemDoubleClick = new RelayCommand<object>(list_Hashtags_MouseDoubleClick);
            OpenTagDetail = new RelayCommand<object>(ExecuteOpenTagDetail);
            OpenSearchResultsCommand = new RelayCommand<object>(ExecuteOpenSearchResults);
            MergeTags = new RelayCommand<object>(ExecuteMergeTags);
        }
        
#region Properties
        public List<OrderByHashTagsOverview> OrderByOptions { get; } =
            Enum.GetValues(typeof(OrderByHashTagsOverview)).Cast<OrderByHashTagsOverview>().ToList();

        public OrderByHashTagsOverview OrderBy
        {
            get => _selectedOrderBy;
            set => SetField(ref _selectedOrderBy, value);
        }
        public ObservableCollection<HashTagModel> IndexingHashtags
        {
            get => _indexingHashtags;
            set
            {
                SetField(ref _indexingHashtags, value);
                OnPropertyChanged(nameof(FilteredIndexingHashtags));
            }
        }

        public ObservableCollection<HashTagModel> FilteredIndexingHashtags => FilterIndexingResults(_indexingHashtags);

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
            set => SetField(ref _minOccurrences, value);
        }
        
        public int MaxOccurrences
        {
            get => _maxOccurrences;
            set => SetField(ref _maxOccurrences, value);
        }
        
        public string SearchBar
        {
            get => _searchBar;
            set
            {
                SetField(ref _searchBar, value);
                OnPropertyChanged(nameof(FilteredIndexingHashtags));
            }
        }

        #endregion

#region Commands
        public ICommand StartIndexingCommand { get; private set; }
        public ICommand HashTagItemDoubleClick { get; private set; }
        public ICommand OrderByChangedCommand { get; private set; }
        public ICommand OpenTagDetail { get; private set; }
        public ICommand OpenSearchResultsCommand { get; private set; }
        public ICommand MergeTags { get; private set; }

#endregion

        private ObservableCollection<HashTagModel> FilterIndexingResults(ObservableCollection<HashTagModel> indexingHashtags)
        {
            if (string.IsNullOrWhiteSpace(SearchBar))
            {
                return indexingHashtags;
            }

            var result = indexingHashtags.Where(x => x.Tag.Contains(SearchBar));
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
            IndexingHashtags = new ObservableCollection<HashTagModel>(result);
        }
        private void UpdateIndexingResults()
        {
            var hashTags = _cache.GetHashTags();
            SetIndexingResult(hashTags.ToList());
        }
        //TODO: FIx this - currently will delete cached need to implement a caching service
        private void IndexingFilrerByNumOfOccurencesMin(int minOccurences)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.Where(x => x.NumOfOccurrences >= minOccurences));
        }

        private void IndexingFilrerByNumOfOccurencesMax(int maxOccurences)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.Where(x => x.NumOfOccurrences <= maxOccurences));
        }
        
        private void IndexingOrderBy(int orderBy)
        {
            //TODO: Implement sorting
            switch (orderBy)
            {
                case (int)OrderByHashTagsOverview.DateDesc:
                    //_indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.));
                    break;
                case (int)OrderByHashTagsOverview.OccurrencesDesc:
                    _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.NumOfOccurrences));
                    break;
                case (int)OrderByHashTagsOverview.OccurrencesAsc:
                    _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderBy(x => x.NumOfOccurrences));
                    break;
                default:
                    break;
            }

        }
        
        
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

        }

        private void OrderByChanged()
        {
            //IndexingOrderBy(index_cb_order_by.SelectedIndex);
        }

        private void MenuItem_Merge_Click(object sender, RoutedEventArgs e)
        {/*
            Dictionary<string, ClusteringSettingDto> clusteringSettings;
            var primaryTag = (HashTagModel)sender;
            var secondaryTags = list_Hashtags.SelectedItems.Cast<HashTagModel>().ToList();
            //Merge the tags
*/

        }

        //Not used
        private void MenuItem_Details_Click(object sender, RoutedEventArgs e)
        {
            var menuItem = sender as System.Windows.Controls.MenuItem;
            var selectedHashTag = (HashTagModel)menuItem.DataContext;
            
            var evt = new ChangeTabEvent(ChangeTabEventTarget.TagDetailsTab, selectedHashTag);
            _eventAggregator.FireEvent(ChangeTabEvent.Tag, evt);
        }

        #endregion
    }
}