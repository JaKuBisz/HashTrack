using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Models.Search;
using HashTrack.UI.ViewModels;
using ListBox = System.Windows.Controls.ListBox;

namespace HashTrack
{
    /// <summary>
    ///     Interaction logic for SidePanelWpfControl.xaml
    /// </summary>
    [RegisterService(LifeCycle.Singleton, typeof(SidePanelWpfControl))]
    public partial class SidePanelWpfControl : UserControl
    {
        public SidePanelWpfControl(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;

            InitializeComponent();

            DataContext = MainViewModel;
        }

        public MainViewModel MainViewModel { get; }

        #region SearchTab

        private void list_searchResults_item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            var item = sender as ListViewItem;
            if (item.Content is ArtefactModel content) ExecuteCommand(MainViewModel.SearchVM.OpenArtefact, content);
        }

        #endregion

        #region HashTagDetailTab

        private void Selector_OnSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var tagsListBox = (ListBox)sender;
            var selectedTags =
                tagsListBox.SelectedItems.Cast<HashTagModel>().ToList();
            MainViewModel.HashTagDetailVM.PopupVM.UpdateSelectionCommand.Execute(selectedTags);
        }

        #endregion


        private void ExecuteCommand(ICommand command, object parameter)
        {
            if (command.CanExecute(parameter)) command.Execute(parameter);
        }

        #region HashTagOverviewTab


        private void list_Hashtags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left) return;

            if (sender is ListViewItem listView
                && listView.Content is HashTagViewModel content)
                ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenTagDetail, content.HashTag);
        }

        private void list_Hashtags_contextMenuItem_tagDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var tagVM = (HashTagViewModel)list_Hashtags.SelectedItem;
            if (tagVM == null) return;

            ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenTagDetail, tagVM.HashTag);
        }

        private void list_Hashtags_contextMenuItem_tagOccurrences_OnClick(object sender, RoutedEventArgs e)
        {
            var tagVM = (HashTagViewModel)list_Hashtags.SelectedItem;
            if (tagVM == null) return;

            ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenSearchResultsCommand, tagVM.HashTag);
        }

        #endregion
    }
}