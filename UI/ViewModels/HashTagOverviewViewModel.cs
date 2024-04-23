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

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagOverviewViewModel))]
    public class HashTagOverviewViewModel : BaseViewModel
    {
        // Add properties and commands for displaying an overview of hash tags
        private readonly ICache _cache;
        private readonly IEventAggregator _eventAggregator;
        private ObservableCollection<HashTagModel> _indexingHashtags;

        public ICommand StartIndexingCommand { get; private set; }
        public ICommand HashTagItemDoubleClick { get; private set; }
        public ICommand OrderByChangedCommand { get; private set; }
        public ICommand OpenTagDetail { get; private set; }
        public ICommand MergeTags { get; private set; }

        public HashTagOverviewViewModel(IEventAggregator eventAggregator, ICache cache)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>();
            _eventAggregator = eventAggregator;
            _cache = cache;
            //TODO: Use Async use eventHandler from Outlook Office object
            eventAggregator.Subscribe(Events.IndexingSearchProcessed, UpdateIndexingResults);
            InitializeCommands();
        }


        private void InitializeCommands()
        {
            StartIndexingCommand = new RelayCommand(StartIndexing);
            OrderByChangedCommand = new RelayCommand(OrderByChanged);
            
            HashTagItemDoubleClick = new RelayCommand<object>(list_Hashtags_MouseDoubleClick);
            OpenTagDetail = new RelayCommand<object>(ExecuteOpenTagDetail);
            MergeTags = new RelayCommand<object>(ExecuteMergeTags);
        }
        
        private void ExecuteOpenTagDetail(object obj)
        {
            if (obj is HashTagModel model)
            {
                _eventAggregator.FireEvent(Events.UI.ChangeSelectedTab, (2, model));
            }
        }

        private void ExecuteMergeTags(object obj)
        {
            if (obj is HashTagModel model)
            {
                _eventAggregator.FireEvent(Events.UI.ChangeSelectedTab, (2, model));
            }
        }
        
        private void SetIndexingResult(List<HashTagModel> result)
        {
            _indexingHashtags.Clear();
            result.ForEach(_indexingHashtags.Add);
        }
        private void UpdateIndexingResults()
        {
            var hashTags = _cache.Get<List<HashTagModel>>(Constants.Storage.IndexedHashTags);
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
                case (int)OrderBy.DateDesc:
                    //_indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.));
                    break;
                case (int)OrderBy.OccurencesDesc:
                    _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.NumOfOccurrences));
                    break;
                case (int)OrderBy.OccurencesAsc:
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
                _eventAggregator.FireEvent(Events.UI.ChangeSelectedTab, (0, model.Tag));
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

            _eventAggregator.FireEvent(Events.UI.ChangeSelectedTab, (2, selectedHashTag));
        }

        #endregion
    }
}