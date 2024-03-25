using HashTrack.DTOs;
using HashTrack.Enums;
using HashTrack.Exception;
using HashTrack.Helpers;
using System;
using System.Collections.Generic;
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
    /// Interaction logic for HashTrackSearchWpfControl.xaml haha
    /// </summary>
    [RegisterService(typeof(HashTrackSearchWpfControl), LifeCycle.Singleton)]
    public partial class HashTrackSearchWpfControl : System.Windows.Controls.UserControl
    {
        private List<SearchResultViewItem> _searchResults = new List<SearchResultViewItem>();

        public HashTrackSearchWpfControl()
        {
            InitializeComponent();
            list_searchResults.ItemsSource = _searchResults;
        }

        public void UpdateSearchResults(List<SearchResultViewItem> searchResults)
        {
            _searchResults.Clear();
            _searchResults.AddRange(searchResults);
            list_searchResults.Items.Refresh();
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
        public delegate void SearchEventHandler(AdvancedSearchQueryDto searchQuery);
        // Define an event based on the delegate
        public event SearchEventHandler SearchInitiated;

        // Method to call when search is initiated (e.g., button click)
        protected void OnSearch(AdvancedSearchQueryDto searchQuery)
        {
            SearchInitiated?.Invoke(searchQuery);
        }

        private void btn_search_Click_1(object sender, RoutedEventArgs e)
        {

        }

        private AdvancedSearchQueryDto GetSearchQuery()
        {
            return new AdvancedSearchQueryDto
            {
                Keyword = tb_searchbar.Text,
                Artefacts = EvaluateArtefactsSelection(),
                From = date_from.SelectedDate,
                To = date_to.SelectedDate
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
    }
}
