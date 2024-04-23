using HashTrack.Core.Models.Search;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HashTrack.UI.ViewModels
{
    public class MainViewModel : INotifyPropertyChanged
    {
        private HashTagModel _selectedHashTag;

        public HashTagModel SelectedHashTag
        {
            get { return _selectedHashTag; }
            set
            {
                if (_selectedHashTag != value)
                {
                    _selectedHashTag = value;
                    OnPropertyChanged(nameof(SelectedHashTag));
                }
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

}
