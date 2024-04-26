using System.Collections.ObjectModel;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    public class HashTagViewModel : BaseViewModel
    {
        private HashTagModel _hashTag;
        
        public ObservableCollection<HashTagModel> MergedHashTags => new ObservableCollection<HashTagModel>(_hashTag.MergedHashTags);

        public ObservableCollection<HashTagModel> ExcludedHashTags => new ObservableCollection<HashTagModel>(_hashTag.ExcludedHashTags);

        public HashTagModel HashTag
        {
            get => _hashTag;
            set => SetField(ref _hashTag, value);
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
        
        
    }
}