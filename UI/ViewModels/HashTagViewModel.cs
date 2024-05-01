using System.Collections.ObjectModel;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(HashTagViewModel))]
    public class HashTagViewModel : BaseViewModel
    {
        private HashTagModel _hashTag;

        public HashTagViewModel()
        {
            
        }

        public HashTagViewModel(HashTagModel hashTag)
        {
            HashTag = hashTag;
        }
        
        public HashTagModel HashTag
        {
            get => _hashTag;
            set
            {
                SetField(ref _hashTag, value);
                OnPropertyChanged(nameof(MergedHashTags));
                OnPropertyChanged(nameof(ExcludedHashTags));
                OnPropertyChanged(nameof(TotalMergedHashTags));
                OnPropertyChanged(nameof(TotalExcludedHashTags));
                OnPropertyChanged(nameof(TotalNumOfOccurrences));
                OnPropertyChanged(nameof(NumOfOccurrences));
                OnPropertyChanged(nameof(CategoryFolderName));
            }
        }
        
        public ObservableCollection<HashTagModel> MergedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.MergedHashTags == null)
                    return new ObservableCollection<HashTagModel>();
                return new ObservableCollection<HashTagModel>(_hashTag.MergedHashTags);
            }
        }

        public ObservableCollection<HashTagModel> TotalMergedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.TotalMergedHashTags() == null)
                    return new ObservableCollection<HashTagModel>();
                return new ObservableCollection<HashTagModel>(_hashTag.TotalMergedHashTags());
            }
        }

        public ObservableCollection<HashTagModel> TotalExcludedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.TotalExcludedHashTags() == null)
                    return new ObservableCollection<HashTagModel>();
                return new ObservableCollection<HashTagModel>(_hashTag.TotalExcludedHashTags());
            }
        }
        
        public ObservableCollection<HashTagModel> ExcludedHashTags
        {
            get
            {
                if (_hashTag == null || _hashTag.ExcludedHashTags == null)
                    return new ObservableCollection<HashTagModel>();
                return new ObservableCollection<HashTagModel>(_hashTag.ExcludedHashTags);
            }
        }
        
        public int TotalNumOfOccurrences
        {
            get
            {
                if (_hashTag == null)
                    return 0;
                return _hashTag.TotalNumOfOccurences();
            }
        }

        public int NumOfOccurrences
        {
            get => _hashTag.NumOfOccurrences;
            set
            {
                _hashTag.NumOfOccurrences = value;
                OnPropertyChanged(nameof(HashTag));
            }
        }
        
        public string CategoryFolderName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(HashTag?.CategoryName)) return "Not enabled";
                return HashTag?.CategoryName;
            }
        }
    }
}