using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.BusinessLogic.Extensions;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;
using HashTrack.Extensions;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagDetailViewModel))]
    public class HashTagDetailViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private readonly IPersistenceHashTagService _persistenceHashTagService;
        private PopupViewModel _popupVm;
        private HashTagModel _hashTag;
        private TagSettingsViewModel _tagSettingsVM;
        
        public ICommand UnmergeCommand { get; private set; }
        public ICommand RemoveExceptionCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }
        public ICommand OpenSettingCommand { get; private set; }
        public ICommand OpenPopupCommand { get; private set; }
        public ICommand ClosePopupCommand { get; private set; }
        public ICommand ConfirmPopupCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        

        public HashTagDetailViewModel(ICache cache, IEventAggregator eventAggregator,
            TagSettingsViewModel tagSettingsVM, IPersistenceHashTagService persistenceHashTagService)
        {
            PopupVM = new PopupViewModel(this);
            _cache = cache;
            _tagSettingsVM = tagSettingsVM;
            _persistenceHashTagService = persistenceHashTagService;
            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
            InitializeCommands();
        }
        
        public ObservableCollection<HashTagModel> MergedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.MergedHashTags == null)
                {
                    return new ObservableCollection<HashTagModel>();
                }
                return new ObservableCollection<HashTagModel>(_hashTag.MergedHashTags);
            }
        }

        public ObservableCollection<HashTagModel> ExcludedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.ExcludedHashTags == null)
                {
                    return new ObservableCollection<HashTagModel>();
                }
                return new ObservableCollection<HashTagModel>(_hashTag.ExcludedHashTags);
            }
        }

        public bool IsEnabled => HashTag != null;

        public HashTagModel HashTag
        {
            get => _hashTag;
            set
            {
                SetField(ref _hashTag, value);
                OnPropertyChanged(nameof(MergedHashTags));
                OnPropertyChanged(nameof(ExcludedHashTags));
                OnPropertyChanged(nameof(IsEnabled));
            }
        }
        
        public string CategoryFolderName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HashTag?.CategoryName))
                {
                    return "Not enabled";
                }
                return HashTag?.CategoryName;
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
            UnmergeCommand = new RelayCommand<HashTagModel>(ExecuteUnmerge);
            RemoveExceptionCommand = new RelayCommand<HashTagModel>(ExecuteRemoveException);
            MergeCommand = new RelayCommand<HashTagModel>(ExecuteMerge);
            OpenPopupCommand = new RelayCommand<object>(ExecuteOpenPopup);
            OpenSettingCommand = new RelayCommand(ExecuteOpenSetting);
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
        
        private void ExecuteOpenSetting()
        {
            _tagSettingsVM.ShowSettings(HashTag);
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

        private void ExecuteUnmerge(HashTagModel hashtag)
        {
            _hashTag.UnMergeHashTag(hashtag);
            HashTag = HashTag;
            _persistenceHashTagService.SaveHashTag(HashTag);

        }

        private void ExecuteRemoveException(HashTagModel hashtag)
        {
            _hashTag.RemoveExcluded(hashtag);
            HashTag = HashTag;
            _persistenceHashTagService.SaveHashTag(HashTag);
        }

        private void ExecuteMerge(HashTagModel hashtag)
        {
            
            _hashTag.MergeHashTag(hashtag);
            HashTag = HashTag;
            _persistenceHashTagService.SaveHashTag(HashTag);
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

                var result = tags.Where(x => x.Id.Contains(SearchTag));
                return new ObservableCollection<HashTagModel>(result);
            }

            private void ClosePopup()
            {
                IsPopupOpen = false;
            }

            private void ConfirmPopup()
            {
                //TODO: do in parent via command or event
                if (IsMergeMode)
                {
                    foreach (var tag in SelectedTags)
                    {
                        _parent._hashTag.MergeHashTag(tag);
                        _parent.HashTag = _parent.HashTag; // Ensure UI update
                    }
                }
                else
                {
                    foreach (var tag in SelectedTags)
                    {
                        _parent._hashTag.UnMergeHashTag(tag);
                        _parent.HashTag = _parent.HashTag;
                    }
                }
                _parent._persistenceHashTagService.SaveHashTag(_parent.HashTag);
                IsPopupOpen = false;
            }
        }
    }
}