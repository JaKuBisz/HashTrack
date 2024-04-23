using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Input;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Transient, typeof(HashTagDetailViewModel))]
public class HashTagDetailViewModel : BaseViewModel
{
    private HashTagModel _hashTag;
    private ObservableCollection<HashTagModel> _mergedHashTags;
    private ObservableCollection<HashTagModel> _excludedHashTags;
    private readonly ICache _cache;
    public ICommand UnmergeCommand { get; private set; }
    public ICommand RemoveExceptionCommand { get; private set; }
    public ICommand MergeCommand { get; private set; }
            

        public HashTagDetailViewModel(ICache cache)
    {
        _cache = cache;
        MergedHashTags = new ObservableCollection<HashTagModel>();
        ExcludedHashTags = new ObservableCollection<HashTagModel>();
        InitializeCommands();

            _hashTag = new HashTagModel
            {
                Tag = "#test",
                NumOfOccurences = 100
            };

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
        UnmergeCommand = new RelayCommand(ExecuteUnmerge, CanExecuteUnmerge);
        RemoveExceptionCommand = new RelayCommand(ExecuteRemoveException, CanExecuteRemoveException);
        MergeCommand = new RelayCommand(ExecuteMerge, CanExecuteMerge);
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

    protected virtual void OnPropertyChanged(string propertyName)
    {
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
    }

    public event PropertyChangedEventHandler PropertyChanged;
}

}