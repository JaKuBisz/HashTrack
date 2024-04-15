using HashTrack.DTOs;
using HashTrack.Exception;
using HashTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HashTrack.Clustering.DTOs;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Persistence;
using HashTrack.Core.Models.Search;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack
{
    /// <summary>
    /// Interaction logic for SidePanelWpfControl.xaml haha
    /// </summary>
    [RegisterService(LifeCycle.Singleton, typeof(SidePanelWpfControl))]
    public partial class SidePanelWpfControl : System.Windows.Controls.UserControl
    {
        private ObservableCollection<ArtefactModel> _searchResults = new ObservableCollection<ArtefactModel>();
        private ObservableCollection<HashTagModel> _indexingHashtags = new ObservableCollection<HashTagModel>();
        private readonly IPersistenceHashTagService _hashTagsStorageService;
        private readonly ICache<List<ArtefactModel>> _artefactCache;
        private readonly IEventPublisher _eventPublisher;

        public SidePanelWpfControl(IEventPublisher eventPublisher, IPersistenceHashTagService hashTagsStorageService, ICache<List<ArtefactModel>> artefactCache)
        {
            InitializeComponent();
            list_searchResults.ItemsSource = _searchResults;
            list_Hashtags.ItemsSource = _indexingHashtags;
            _eventPublisher = eventPublisher;
            _hashTagsStorageService = hashTagsStorageService;
            _artefactCache = artefactCache;
            //TODO: Use Async
            eventPublisher.Subscribe(Events.IndexingSearchProcessed, UpdateIndexingResults);
            eventPublisher.Subscribe(Events.DefaultSearchProcessed, UpdateSearchResults);
        }

        public void SetSearchResults(List<ArtefactModel> searchResults)
        {
            _searchResults.Clear();
            searchResults.ForEach(_searchResults.Add);
            IndexingOrderBy(index_cb_order_by.SelectedIndex);
            list_searchResults.Items.Refresh();
        }

        private void SetIndexingResult(List<HashTagModel> result)
        {
            _indexingHashtags.Clear();
            result.ForEach(_indexingHashtags.Add);
            list_Hashtags.Items.Refresh();
        }


        public enum OrderBy
        {
            DateDesc,
            OccurencesDesc,
            OccurencesAsc
        }

        private void UpdateIndexingResults()
        {
            var hashTags = _hashTagsStorageService.GetAllHashTags();
            SetIndexingResult(hashTags.ToList());
        }
        

        private void UpdateSearchResults()
        {
            //TODO: Replace this by better system to know the order of searches and so on; so they can be set from other services also
            var hashTag = tb_searchbar.Text;
            var artefacts = _artefactCache.Get(hashTag);
            SetSearchResults(artefacts);
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
                Keyword = tb_searchbar.Text,
                Artefacts = EvaluateArtefactsSelection(),
                From = date_from.SelectedDate,
                To = date_to.SelectedDate,
                Tag = Events.DefaultSearchCompleted,
                ExactMatch = true
            };
        }

        private ArtifactTypes EvaluateArtefactsSelection()
        {
            var artefacts = ArtifactTypes.None;

            if (ch_email.IsChecked == true)
            {
                artefacts |= ArtifactTypes.Email;
            }
            if (ch_appointment.IsChecked == true)
            {
                artefacts |= ArtifactTypes.Appointment;
            }
            if (ch_contact.IsChecked == true)
            {
                artefacts |= ArtifactTypes.Contact;
            }
            if (ch_task.IsChecked == true)
            {
                artefacts |= ArtifactTypes.Task;
            }

            if (artefacts == ArtifactTypes.None)
            {
                throw new SearchQueryException("Please select at least one artefact type to search for.");
            }

            return artefacts;
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
                    _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderByDescending(x => x.NumOfOccurences));
                    break;
                case (int)OrderBy.OccurencesAsc:
                    _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.OrderBy(x => x.NumOfOccurences));
                    break;
                default:
                    break;
            }

        }

        //TODO: FIx this - currently will delete cached need to implement a caching service
        private void IndexingFilrerByNumOfOccurencesMin(int minOccurences)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.Where(x => x.NumOfOccurences >= minOccurences));
        }

        private void IndexingFilrerByNumOfOccurencesMax(int maxOccurences)
        {
            _indexingHashtags = new ObservableCollection<HashTagModel>(_indexingHashtags.Where(x => x.NumOfOccurences <= maxOccurences));
        }

        private void list_searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as System.Windows.Controls.ListView;
            var selectedItem = listView.SelectedItem as ArtefactModel;
            if (selectedItem == null)
            {
                return;
            }

            if (selectedItem.OriginalItem is Outlook.MailItem mailItem)
            {
                mailItem.Display(false);
            }
            else if (selectedItem.OriginalItem is Outlook.AppointmentItem appointmentItem)
            {
                appointmentItem.Display(false);
            }
            else if (selectedItem.OriginalItem is Outlook.ContactItem contactItem)
            {
                contactItem.Display(false);
            }
            else if (selectedItem.OriginalItem is Outlook.TaskItem taskItem)
            {
                taskItem.Display(false);
            }

        }

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

        private void list_Hashtags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var item = sender as System.Windows.Controls.ListViewItem;
            var content = item.Content as HashTagModel;
            if (content == null)
            {
                return;
            }

            var searchResults = content.SearchResults;
            tb_searchbar.Text = content.Id;
            SetSearchResults(searchResults.ToList());
            mainTabControl.SelectedIndex = 0;
        }

        private void StartIndexing_Click(object sender, RoutedEventArgs e)
        {

        }

        private void index_cb_order_by_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            IndexingOrderBy(index_cb_order_by.SelectedIndex);
        }

        private void MenuItem_Merge_Click(object sender, RoutedEventArgs e)
        {
            Dictionary<string, ClusteringSettingDto> clusteringSettings;
            var primaryTag = (HashTagModel)sender;
            var secondaryTags = list_Hashtags.SelectedItems.Cast<HashTagModel>().ToList();
            //Merge the tags


        }

        private void MenuItem_Details_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddMergedTag_Click(object sender, RoutedEventArgs e)
        {
            AddTagPopup.IsOpen = true;
        }

        private void AddExcludedTag_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ClosePopup_Click(object sender, RoutedEventArgs e)
        {

        }

        private void AddTag_Click(object sender, RoutedEventArgs e)
        {

        }
    }
}
