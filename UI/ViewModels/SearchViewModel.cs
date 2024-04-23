using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace HashTrack.UI.ViewModels
{
    public class SearchViewModel : BaseViewModel
    {
        private SearchFilters _searchFilters;
        private ObservableCollection<SearchResult> _searchResults;
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

        public ObservableCollection<SearchResult> SearchResults
        {
            get => _searchResults;
            set => SetField(ref _searchResults, value);
        }

        public ICommand SearchCommand { get; }

        public SearchViewModel()
        {
            SearchResults = new ObservableCollection<SearchResult>();
            SearchFilters = new SearchFilters();
            SearchCommand = new RelayCommand(ExecuteSearch);
        }

        private void ExecuteSearch(object parameter)
        {
            // Implement search logic here
            // Example: Update SearchResults based on SearchText, Date, and other filters
        }
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
    }
}