using HashTrack.Core.Models.Search;
using System;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
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
            HashTagDetailViewModel hashTagDetailVM,
            TagSettingsViewModel hshtagvm)
        {
            HashTagDetailVM = hashTagDetailVM;
            SearchVM = searchVM;
            HashTagOverviewVM = hashTagOverviewVM;

            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
            TabChange = new RelayCommand<object>(ExecuteTabChange);

            var hashtagset = new HashTagSettings();
            hashtagset.DataContext = hshtagvm;
            var test = new Form1(hashtagset);
            test.Show();
        }
        
        public int SelectedTabIndex
        {
            get => _selectedTabIndex;
            set => SetField(ref _selectedTabIndex, value);
        }

        private void ExecuteTabChange(object obj)
        {
            if (!(obj is ChangeTabEvent evt) || evt.TagModel is null)
            {
                return;
            }
/*
            switch(evt.Target)
            {
                case ChangeTabEventTarget.SearchTab:
                    //SearchVM
                    SearchVM.SearchFilters.SearchText = evt.TagModel.Tag;
                    break;
                case ChangeTabEventTarget.TagOverviewTab:
                    //HashTagOverviewVM
                    return;
                    break;
                case ChangeTabEventTarget.TagDetailsTab:
                    //HashTagDetailVM
                    HashTagDetailVM.HashTag = evt.TagModel;
                    break;
                default:
                    return;
            }*/
            
            SelectedTabIndex = (int)evt.Target;
        }
    }


}
