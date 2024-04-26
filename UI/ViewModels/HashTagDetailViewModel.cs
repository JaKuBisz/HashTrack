using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;
using HashTrack.Extensions;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagDetailViewModel))]
    public class HashTagDetailViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private PopupViewModel _popupVm;
        private HashTagModel _hashTag;
        
        public ICommand UnmergeCommand { get; private set; }
        public ICommand RemoveExceptionCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }
        public ICommand OpenPopupCommand { get; private set; }
        public ICommand ClosePopupCommand { get; private set; }
        public ICommand ConfirmPopupCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        

        public HashTagDetailViewModel(ICache cache, IEventAggregator eventAggregator)
        {
            PopupVM = new PopupViewModel(this);
            _cache = cache;
            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
            InitializeCommands();
        }
        
        public ObservableCollection<HashTagModel> MergedHashTags => new ObservableCollection<HashTagModel>(_hashTag.MergedHashTags);

        public ObservableCollection<HashTagModel> ExcludedHashTags => new ObservableCollection<HashTagModel>(_hashTag.ExcludedHashTags);
       
        public HashTagModel HashTag
        {
            get => _hashTag;
            set
            {
                SetField(ref _hashTag, value);
                OnPropertyChanged(nameof(MergedHashTags));
                OnPropertyChanged(nameof(ExcludedHashTags));
            }
        }

        public PopupViewModel PopupVM
        {
            get => _popupVm;
            set => SetField(ref _popupVm, value);
        }

/*
    public HashTagDetailViewModel()
    {
        MergedHashTags = new ObservableCollection<HashTagModel>();
        ExcludedHashTags = new ObservableCollection<HashTagModel>();
        InitializeCommands();
    }*/

        private void InitializeCommands()
        {
            UnmergeCommand = new RelayCommand<object>(ExecuteUnmerge, CanExecuteUnmerge);
            RemoveExceptionCommand = new RelayCommand<object>(ExecuteRemoveException, CanExecuteRemoveException);
            MergeCommand = new RelayCommand<object>(ExecuteMerge, CanExecuteMerge);
            OpenPopupCommand = new RelayCommand<object>(ExecuteOpenPopup);
            AddTagCommand = new RelayCommand(AddTag_Click);
            
            /*
            ClosePopupCommand = new RelayCommand<object>(param => IsPopupOpen = false);
            AddTagCommand = new RelayCommand<object>(param =>
            {
                // Add your tag adding logic here
                IsPopupOpen = false;
            });*/
        }
        
        private void ExecuteTabChange(object obj)
        {
            if (!(obj is ChangeTabEvent evt) || evt.TagModel is null || evt.Target != ChangeTabEventTarget.TagDetailsTab)
            {
                return;
            }
            
            HashTag = evt.TagModel;
        }
        
        private void ExecuteOpenPopup(object parameter)
        {
            //Merged tags
            bool isMerged = Convert.ToBoolean(parameter);

            var tags = _cache.GetHashTags().ToHashSet();
            var exceptTags = isMerged ? _hashTag.TotalMergedHashTags() : _hashTag.TotalExcludedHashTags();
            exceptTags.Add(HashTag);
            
            var resultTags = tags.Except(exceptTags);

            PopupVM.Open(isMerged, new ObservableCollection<HashTagModel>(resultTags));
        }

        private void ExecuteUnmerge(object parameter)
        {
            // Unmerge logic here
        }

        private bool CanExecuteUnmerge(object parameter)
        {
            return true; // Logic to determine if unmerge can be executed
        }

        private void ExecuteRemoveException(object parameter)
        {
            // Remove exception logic here
        }

        private bool CanExecuteRemoveException(object parameter)
        {
            return true; // Logic to determine if removing an exception can be executed
        }

        private void ExecuteMerge(object parameter)
        {
            // Merge logic here
        }

        private bool CanExecuteMerge(object parameter)
        {
            return true; // Logic to determine if merge can be executed
        }

        #region Events

        
        
        private void AddTag_Click()
        {
            //Excluded or Merged?
            /*
            var newTag = new HashTagModel { Tag = SearchTagTextBox.Text };
            HashTagDetailVM.MergedHashTags.Add(newTag);*/
            //AddTagPopup.IsOpen = false;
        }

        private void ContextMenu_SeeDetails_Click(object sender, RoutedEventArgs e)
        {

            var menuItem = sender as System.Windows.Controls.MenuItem;
            /*SelectedHashTag = (HashTagModel)menuItem.DataContext;
            HashTagDetailVM.HashTag = SelectedHashTag;
            mainTabControl.SelectedIndex = 2;*/
        }

        #endregion

        public class PopupViewModel : BaseViewModel
        {
            private readonly HashTagDetailViewModel _parent;
            private ObservableCollection<HashTagModel> _popupTags;
            private List<HashTagModel> _selectedTags;
            private string _searchTag;
            private bool _isPopupOpen;
            private bool _isMergeMode;

            public ICommand ClosePopupCommand { get; private set; }
            public ICommand ConfirmPopupCommand { get; private set; }
            
            public ICommand AddTagCommand { get; private set; }
            public ICommand UpdateSelectionCommand { get; private set; }


            public PopupViewModel(HashTagDetailViewModel parent)
            {
                _parent = parent;
                _popupTags = new ObservableCollection<HashTagModel>();
                InitializeCommands();
            }
            
            private void InitializeCommands()
            {
                ClosePopupCommand = new RelayCommand(ClosePopup);
                ConfirmPopupCommand = new RelayCommand(ConfirmPopup);
                //AddTagCommand = new RelayCommand(AddTag_Click);
                UpdateSelectionCommand = new RelayCommand<List<HashTagModel>>(UpdateSelection);
            }

            private void UpdateSelection(List<HashTagModel> obj)
            {
                if (obj == null)
                {
                    return;
                }
                SelectedTags = obj;
            }

            public void Open(bool isMergeMode, ObservableCollection<HashTagModel> tags)
            {
                IsMergeMode = isMergeMode;
                PopupTags = tags;
                IsPopupOpen = true;
            }

            public string SearchTag
            {
                get => _searchTag;
                set
                {
                    SetField(ref _searchTag, value);
                    OnPropertyChanged(nameof(PopupTags));
                }
            }

            public ObservableCollection<HashTagModel> PopupTags
            {
                get => FilterTags(_popupTags);
                set
                {
                    SetField(ref _popupTags, value);
                }
            }

            public List<HashTagModel> SelectedTags
            {
                get => _selectedTags;
                set => SetField(ref _selectedTags, value);
            }
            //TODO: Fix keeps getting set to false
            public bool IsPopupOpen
            {
                get => _isPopupOpen;
                set => SetField(ref _isPopupOpen, value);
            }

            public bool IsMergeMode
            {
                get => _isMergeMode;
                set => SetField(ref _isMergeMode, value);
            }
            
            private ObservableCollection<HashTagModel> FilterTags(ObservableCollection<HashTagModel> tags)
            {
                if (string.IsNullOrWhiteSpace(SearchTag))
                {
                    return tags;
                }

                var result = tags.Where(x => x.Tag.Contains(SearchTag));
                return new ObservableCollection<HashTagModel>(result);
            }

            private void ClosePopup()
            {
                IsPopupOpen = false;
            }

            private void ConfirmPopup()
            {
                //TODO: implement persistent merge
                if (IsMergeMode)
                {
                    foreach (var tag in PopupTags)
                    {
                        //_parent._mergedHashTags.Add(tag);
                    }
                }
                else
                {
                    foreach (var tag in PopupTags)
                    {
                       // _parent._excludedHashTags.Add(tag);
                    }
                }
                IsPopupOpen = false;
            }
        }
    }
}