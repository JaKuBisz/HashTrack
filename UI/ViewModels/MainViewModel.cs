using HashTrack.Core.Models.Search;
using System;
using System.Windows.Input;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(MainViewModel))]
    public class MainViewModel : BaseViewModel
    {
        private int _selectedTabIndex;
        public HashTagDetailViewModel HashTagDetailVM { get; private set; }
        public SearchViewModel SearchVM { get; private set; }
        public HashTagOverviewViewModel HashTagOverviewVM { get; private set; }
        public ICommand TabChange { get; private set; }

        public MainViewModel(
            IEventAggregator eventAggregator,
            SearchViewModel searchVM,
            HashTagOverviewViewModel hashTagOverviewVM,
            HashTagDetailViewModel hashTagDetailVM)
        {
            HashTagDetailVM = hashTagDetailVM;
            SearchVM = searchVM;
            HashTagOverviewVM = hashTagOverviewVM;

            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
        }
        
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetField(ref _selectedTabIndex, value);
        }

        private void ExecuteTabChange(object obj)
        {
            if (!(obj is Tuple<int, object> tuple))
            {
                return;
            }
            
            var tabIndex = tuple.Item1;
            var data = tuple.Item2;
            switch(tabIndex)
            {
                case 0:
                    //SearchVM
                    if (data is string searchQuery)
                    {
                        SearchVM.SearchFilters.SearchText = searchQuery;
                    }
                    break;
                case 1:
                    //HashTagOverviewVM
                    break;
                case 2:
                    //HashTagDetailVM
                    if (data is HashTagModel hashTag)
                    {
                        HashTagDetailVM.HashTag = hashTag;
                    }
                    break;
                default:
                    return;
            }
            
            SelectedTabIndex = tabIndex;
        }
    }


}
