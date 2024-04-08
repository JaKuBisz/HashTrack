using HashTrack.DTOs;
using HashTrack.Enums;
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
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack
{
    /// <summary>
    /// Interaction logic for SidePanelWpfControl.xaml haha
    /// </summary>
    [RegisterService(typeof(SidePanelWpfControl), LifeCycle.Singleton)]
    public partial class SidePanelWpfControl : System.Windows.Controls.UserControl
    {
        private ObservableCollection<SearchResultViewItem> _searchResults = new ObservableCollection<SearchResultViewItem>();
        private ObservableCollection<IndexingResultsViewItem> _indexingHashtags = new ObservableCollection<IndexingResultsViewItem>();

        public SidePanelWpfControl()
        {
            InitializeComponent();
            list_searchResults.ItemsSource = _searchResults;
            list_Hashtags.ItemsSource = _indexingHashtags;
        }

        public void SetSearchResults(List<SearchResultViewItem> searchResults)
        {
            _searchResults.Clear();
            searchResults.ForEach(_searchResults.Add);
            IndexingOrderBy(index_cb_order_by.SelectedIndex);
            list_searchResults.Items.Refresh();
        }

        public void SetIndexingResult(List<IndexingResultsViewItem> result)
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
                Tag = Constants.DefaultSearchTag,
                ExactMatch = true
            };
        }

        private ArtifactTypes EvaluateArtefactsSelection()
        {
            var artefacts = Enums.ArtifactTypes.None;

            if (ch_email.IsChecked == true)
            {
                artefacts |= Enums.ArtifactTypes.Email;
            }
            if (ch_appointment.IsChecked == true)
            {
                artefacts |= Enums.ArtifactTypes.Appointment;
            }
            if (ch_contact.IsChecked == true)
            {
                artefacts |= Enums.ArtifactTypes.Contact;
            }
            if (ch_task.IsChecked == true)
            {
                artefacts |= Enums.ArtifactTypes.Task;
            }

            if (artefacts == Enums.ArtifactTypes.None)
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
                    //_indexingHashtags = new ObservableCollection<IndexingResultsViewItem>(_indexingHashtags.OrderByDescending(x => x.));
                    break;
                case (int)OrderBy.OccurencesDesc:
                    _indexingHashtags = new ObservableCollection<IndexingResultsViewItem>(_indexingHashtags.OrderByDescending(x => x.NumOfOccurences));
                    break;
                case (int)OrderBy.OccurencesAsc:
                    _indexingHashtags = new ObservableCollection<IndexingResultsViewItem>(_indexingHashtags.OrderBy(x => x.NumOfOccurences));
                    break;
                default:
                    break;
            }

        }

        //TODO: FIx this - currently will delete cached need to implement a caching service
        private void IndexingFilrerByNumOfOccurencesMin(int minOccurences)
        {
            _indexingHashtags = new ObservableCollection<IndexingResultsViewItem>(_indexingHashtags.Where(x => x.NumOfOccurences >= minOccurences));
        }

        private void IndexingFilrerByNumOfOccurencesMax(int maxOccurences)
        {
            _indexingHashtags = new ObservableCollection<IndexingResultsViewItem>(_indexingHashtags.Where(x => x.NumOfOccurences <= maxOccurences));
        }

        private void list_searchResults_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var listView = sender as System.Windows.Controls.ListView;
            var selectedItem = listView.SelectedItem as SearchResultViewItem;
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
            var item = sender as System.Windows.Controls.ListViewItem;
            var content = item.Content as SearchResultViewItem;
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
            var item = sender as System.Windows.Controls.ListViewItem;
            var content = item.Content as IndexingResultsViewItem;
            if (content == null)
            {
                return;
            }

            var searchResults = content.SearchResults;
            tb_searchbar.Text = content.HashTag;
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
    }
}
