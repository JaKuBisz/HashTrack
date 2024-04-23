using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using System.Windows.Input;
using CommunityToolkit.Mvvm.Input;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagDetailViewModel))]
    public class HashTagDetailViewModel : BaseViewModel
    {
        private HashTagModel _hashTag;
        private ObservableCollection<HashTagModel> _mergedHashTags;
        private ObservableCollection<HashTagModel> _excludedHashTags;
        private bool _isPopupOpen;
        private readonly ICache _cache;
        public ICommand UnmergeCommand { get; private set; }
        public ICommand RemoveExceptionCommand { get; private set; }
        public ICommand MergeCommand { get; private set; }
        public ICommand OpenPopupCommand { get; private set; }
        public ICommand ClosePopupCommand { get; private set; }
        public ICommand ConfirmPopupCommand { get; private set; }
        public ICommand AddTagCommand { get; private set; }
        



        public HashTagDetailViewModel(ICache cache)
        {
            _cache = cache;
            MergedHashTags = new ObservableCollection<HashTagModel>();
            ExcludedHashTags = new ObservableCollection<HashTagModel>();
            InitializeCommands();

            _hashTag = new HashTagModel
            {
                Tag = "#test",
                NumOfOccurrences = 100
            };
        }

        public bool IsPopupOpen
        {
            get => _isPopupOpen;
            set => SetField(ref _isPopupOpen, value);
        }

        public ObservableCollection<HashTagModel> MergedHashTags
        {
            get => _mergedHashTags;
            set => SetField(ref _mergedHashTags, value);
        }

        public ObservableCollection<HashTagModel> ExcludedHashTags
        {
            get => _excludedHashTags;
            set => SetField(ref _excludedHashTags, value);
        }

        public HashTagModel HashTag
        {
            get => _hashTag;
            set => SetField(ref _hashTag, value);
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
            
            ClosePopupCommand = new RelayCommand<object>(param => IsPopupOpen = false);
            AddTagCommand = new RelayCommand<object>(param =>
            {
                // Add your tag adding logic here
                IsPopupOpen = false;
            });
        }
        
        private void ExecuteOpenPopup(object parameter)
        {
            //Merged tags
            bool isMerged = Convert.ToBoolean(parameter);

            IsPopupOpen = true;
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
    }
}