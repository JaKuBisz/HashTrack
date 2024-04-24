using HashTrack.DTOs;
using HashTrack.Exception;
using HashTrack.Helpers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using HashTrack.Clustering.DTOs;
using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Persistence;
using HashTrack.Core.Models.Search;
using HashTrack.UI.ViewModels;
using ComboBox = System.Windows.Controls.ComboBox;
using ListBox = System.Windows.Controls.ListBox;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack
{
    /// <summary>
    /// Interaction logic for SidePanelWpfControl.xaml
    /// </summary>
    [RegisterService(LifeCycle.Singleton, typeof(SidePanelWpfControl))]
    public partial class SidePanelWpfControl : System.Windows.Controls.UserControl
    {
        public MainViewModel MainViewModel { get; private set; }

        public SidePanelWpfControl(MainViewModel mainViewModel)
        {
            MainViewModel = mainViewModel;

            InitializeComponent();
            
            this.DataContext = MainViewModel;
        }

        #region SearchTab
        
        private void list_searchResults_item_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }

            var item = sender as System.Windows.Controls.ListViewItem;
            if (item.Content is ArtefactModel content)
            {
                ExecuteCommand(MainViewModel.SearchVM.OpenArtefact, content);
            }
        }
        
        #endregion
        
        #region HashTagOverviewTab
        
        private void index_cb_order_by_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox comboBox)
            {
                ExecuteCommand(MainViewModel.HashTagOverviewVM.OrderByChangedCommand, comboBox.SelectedIndex);
            }
            
        }
        
        private void list_Hashtags_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton != MouseButton.Left)
            {
                return;
            }
            
            if (sender is System.Windows.Controls.ListViewItem listView
                && listView.Content is HashTagModel content)
            {
                ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenTagDetail, content);
            }
        }

        private void list_Hashtags_contextMenuItem_tagDetails_OnClick(object sender, RoutedEventArgs e)
        {
            var tag = (HashTagModel)list_Hashtags.SelectedItem;
            if (tag == null)
            {
                return;
            }
            
            ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenTagDetail, tag);
        }

        private void list_Hashtags_contextMenuItem_tagOccurrences_OnClick(object sender, RoutedEventArgs e)
        {
            var tag = (HashTagModel)list_Hashtags.SelectedItem;
            if (tag == null)
            {
                return;
            }
            
            ExecuteCommand(MainViewModel.HashTagOverviewVM.OpenSearchResultsCommand, tag);
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
            if (command.CanExecute(parameter))
            {
                command.Execute(parameter);
            }
        }
    }
}
