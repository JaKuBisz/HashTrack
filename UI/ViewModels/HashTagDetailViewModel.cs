using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Extensions;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagDetailViewModel))]
    public class HashTagDetailViewModel : BaseViewModel
    {
        private readonly ICache _cache;
        private readonly IPersistenceHashTagService _persistenceHashTagService;
        private readonly TagSettingsViewModel _tagSettingsVM;
        private readonly IEventAggregator _eventAggregator;
        
        private HashTagViewModel _hashTagVM;
        private PopupViewModel _popupVm;


        public HashTagDetailViewModel(ICache cache, IEventAggregator eventAggregator,
            TagSettingsViewModel tagSettingsVM, HashTagViewModel hashTagVM,
            IPersistenceHashTagService persistenceHashTagService)
        {
            PopupVM = new PopupViewModel(this);
            _cache = cache;
            _tagSettingsVM = tagSettingsVM;
            _hashTagVM = hashTagVM;
            _persistenceHashTagService = persistenceHashTagService;
            _eventAggregator = eventAggregator;
            eventAggregator.Subscribe(Events.UI.ChangeSelectedTab, ExecuteTabChange);
            InitializeCommands();
        }

        public ICommand UnmergeCommand { get; private set; }
        public ICommand RemoveExceptionCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }
        public ICommand OpenSettingCommand { get; private set; }
        public ICommand OpenPopupCommand { get; private set; }

        public bool IsEnabled => _hashTagVM.HashTag != null;

        public HashTagViewModel HashTagVM
        {
            get => _hashTagVM;
            set
            {
                SetField(ref _hashTagVM, value);
                OnPropertyChanged(nameof(IsEnabled));
            }
        }

        public string CategoryFolderName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HashTagVM.HashTag?.CategoryName)) return "Not enabled";
                return HashTagVM.HashTag?.CategoryName;
            }
        }

        public PopupViewModel PopupVM
        {
            get => _popupVm;
            set => SetField(ref _popupVm, value);
        }

        private void InitializeCommands()
        {
            UnmergeCommand = new RelayCommand<HashTagModel>(ExecuteUnmerge);
            RemoveExceptionCommand = new RelayCommand<HashTagModel>(ExecuteRemoveException);
            MergeCommand = new RelayCommand<HashTagModel>(ExecuteMerge);
            OpenPopupCommand = new RelayCommand<object>(ExecuteOpenPopup);
            OpenSettingCommand = new RelayCommand(ExecuteOpenSetting);
        }

        private void ExecuteTabChange(object obj)
        {
            if (!(obj is ChangeTabEvent evt) || evt.TagModel is null ||
                evt.Target != ChangeTabEventTarget.TagDetailsTab) return;

            HashTagVM.HashTag = evt.TagModel;
            HashTagVM = HashTagVM;
        }

        private void ExecuteOpenSetting()
        {
            _tagSettingsVM.ShowSettings(HashTagVM.HashTag);
        }

        private void ExecuteOpenPopup(object parameter)
        {
            //Merged tags
            var isMerged = Convert.ToBoolean(parameter);

            var tags = _cache.GetHashTags().ToHashSet();
            var exceptTags = isMerged ? HashTagVM.HashTag.TotalMergedHashTags() : HashTagVM.HashTag.TotalExcludedHashTags();
            exceptTags.Add(HashTagVM.HashTag);

            var resultTags = tags.Except(exceptTags);

            PopupVM.Open(isMerged, new ObservableCollection<HashTagModel>(resultTags));
        }
        
        private void SaveHashTag(HashTagModel hashtag)
        {
            HashTagVM.HashTag = HashTagVM.HashTag;
            _persistenceHashTagService.SaveHashTag(hashtag);
            _eventAggregator.FireEvent(Events.HashTagsUpdated);
        }

        private void ExecuteUnmerge(HashTagModel hashtag)
        {
            HashTagVM.HashTag.UnMergeHashTag(hashtag);
            SaveHashTag(HashTagVM.HashTag);
        }

        private void ExecuteRemoveException(HashTagModel hashtag)
        {
            HashTagVM.HashTag.RemoveExcluded(hashtag);
            SaveHashTag(HashTagVM.HashTag);
        }

        private void ExecuteMerge(HashTagModel hashtag)
        {
            HashTagVM.HashTag.MergeHashTag(hashtag);
            SaveHashTag(HashTagVM.HashTag);
        }

        public class PopupViewModel : BaseViewModel
        {
            private readonly HashTagDetailViewModel _parent;
            private bool _isMergeMode;
            private bool _isPopupOpen;
            private ObservableCollection<HashTagModel> _popupTags;
            private string _searchTag;
            private List<HashTagModel> _selectedTags;


            public PopupViewModel(HashTagDetailViewModel parent)
            {
                _parent = parent;
                _popupTags = new ObservableCollection<HashTagModel>();
                InitializeCommands();
            }

            public ICommand ClosePopupCommand { get; private set; }
            public ICommand ConfirmPopupCommand { get; private set; }

            public ICommand AddTagCommand { get; }
            public ICommand UpdateSelectionCommand { get; private set; }

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
                set => SetField(ref _popupTags, value);
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

            private void InitializeCommands()
            {
                ClosePopupCommand = new RelayCommand(ClosePopup);
                ConfirmPopupCommand = new RelayCommand(ConfirmPopup);
                UpdateSelectionCommand = new RelayCommand<List<HashTagModel>>(UpdateSelection);
            }

            private void UpdateSelection(List<HashTagModel> obj)
            {
                if (obj == null) return;
                SelectedTags = obj;
            }

            public void Open(bool isMergeMode, ObservableCollection<HashTagModel> tags)
            {
                IsMergeMode = isMergeMode;
                PopupTags = tags;
                IsPopupOpen = true;
            }

            private ObservableCollection<HashTagModel> FilterTags(ObservableCollection<HashTagModel> tags)
            {
                if (string.IsNullOrWhiteSpace(SearchTag)) return tags;

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
                    foreach (var tag in SelectedTags)
                    {
                        _parent.HashTagVM.HashTag.MergeHashTag(tag);
                    }
                else
                    foreach (var tag in SelectedTags)
                    {
                        _parent.HashTagVM.HashTag.UnMergeHashTag(tag);
                    }
                
                _parent.SaveHashTag(_parent.HashTagVM.HashTag);
                IsPopupOpen = false;
            }
        }
    }
}