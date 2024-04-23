using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Windows;
using System.Windows.Forms;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;
using HashTrack.Exception;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(SearchViewModel))]
    public class SearchViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private readonly IEventAggregator _eventAggregator;
        private SearchFilters _searchFilters;
        private ObservableCollection<ArtefactModel> _searchResults;
        private string _orderBy = "Date";

        public string OrderBy
        {
            get => _orderBy;
            set => SetField(ref _orderBy, value);
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

        public ICommand SearchCommand { get; }
        public ICommand OpenArtefact { get; }

        public SearchViewModel(IEventAggregator eventAggregator, ICache cache)
        {
            _eventAggregator = eventAggregator;
            _cache = cache;
            eventAggregator.Subscribe(Events.DefaultSearchProcessed, UpdateSearchResults);
            SearchResults = new ObservableCollection<ArtefactModel>();
            SearchFilters = new SearchFilters();
            SearchCommand = new RelayCommand(ExecuteSearch);
        }
        
        public void SetSearchResults(List<ArtefactModel> searchResults)
        {
            _searchResults.Clear();
            searchResults.ForEach(_searchResults.Add);
            //IndexingOrderBy(index_cb_order_by.SelectedIndex);
            OnPropertyChanged(nameof(SearchResults));
            //list_searchResults.Items.Refresh();
        }        
        
        private void UpdateSearchResults()
        {
            //TODO: Replace this by better system to know the order of searches and so on; so they can be set from other services also
            //var hashTag = tb_searchbar.Text;
            var artefacts = _cache.Get<List<ArtefactModel>>(Constants.Storage.Artefacts);
            SetSearchResults(artefacts);
        }

        public void ExecuteSearch()
        {
            //TODO: Add extension to verify search query
            _eventAggregator.FireEvent(Events.DefaultSearchInitiated, GetSearchQuery());
        }
        
        private void IndexingOrderBy(int orderBy)
        {/*
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
*/
        }
        
        
        private void btn_search_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                var searchQuery = GetSearchQuery();
                OnSearch(searchQuery);
            }
            catch (SearchQueryException ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Search query is incorrect", MessageBoxButtons.OK, MessageBoxIcon.Warning);
            }
            catch (System.Exception ex)
            {
                System.Windows.Forms.MessageBox.Show(ex.Message, "Unhandled exception eccured", MessageBoxButtons.OK, MessageBoxIcon.Error);
                throw;
            }
        }

        #region Search
        // Define a delegate for search event
        public delegate void SearchEventHandler(AdvancedSearchQueryOptions searchQuery);
        // Define an event based on the delegate
        public event SearchEventHandler SearchInitiated;

        // Method to call when search is initiated (e.g., button click)
        protected void OnSearch(AdvancedSearchQueryOptions searchQuery)
        {
            SearchInitiated?.Invoke(searchQuery);
        }

        private void btn_search_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private AdvancedSearchQueryOptions GetSearchQuery()
        {
            return new AdvancedSearchQueryOptions
            {
                Keyword = SearchFilters.SearchText,
                Artefacts = SearchFilters.EvaluateArtefactsSelection(),
                From = SearchFilters.FromDate,
                To = SearchFilters.ToDate,
                Tag = Events.DefaultSearchCompleted,
                ExactMatch = true,
            };
        }

        #endregion

        #region Events

        
        private void list_searchResults_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var item = sender as System.Windows.Controls.ListViewItem;
            var content = item.Content as ArtefactModel;
            if (content == null)
            {
                return;
            }

            if (content.OriginalItem is Outlook.MailItem mailItem)
            {
                mailItem.Display(false);
            }
            else if (content.OriginalItem is Outlook.AppointmentItem appointmentItem)
            {
                appointmentItem.Display(false);
            }
            else if (content.OriginalItem is Outlook.ContactItem contactItem)
            {
                contactItem.Display(false);
            }
            else if (content.OriginalItem is Outlook.TaskItem taskItem)
            {
                taskItem.Display(false);
            }

        }

        #endregion
    }
    
    public class SearchResult
    {
        public string Title { get; set; }
        public string Sender { get; set; }
        public DateTime Date { get; set; }
        public string Type { get; set; }
    }

    public class SearchFilters
    {
        public bool IsEmailChecked { get; set; }
        public bool IsAppointmentsChecked { get; set; }
        public bool IsTasksChecked { get; set; }
        public bool IsContactsChecked { get; set; }
        public DateTime? FromDate { get; set; }
        public DateTime? ToDate { get; set; }
        public string SearchText { get; set; }
        
        
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

            if (artefacts == ArtifactTypes.None)
            {
                throw new SearchQueryException("Please select at least one artefact type to search for.");
            }
            return artefacts;
        }
    }
}