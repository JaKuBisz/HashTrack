using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows.Input;
using Microsoft.Office.Interop.Outlook;
using CommunityToolkit.Mvvm.Input;
using HashTrack.BusinessLogic.Services;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Persistence;
using HashTrack.Core.Models.Search;

namespace HashTrack.UI.ViewModels
{
    [RegisterService(LifeCycle.Singleton, typeof(TagSettingsViewModel))]
    public class TagSettingsViewModel : BaseViewModel
    {
        private readonly IPersistenceHashTagService _storage;
        private readonly ICategoryManagerService _categoryManager;
        private readonly Form1 _popupForm;
        private HashTagModel _hashTag;
        private bool _createFolderEnabled;
        private bool _createCategoryEnabled;
        private string _folderName;
        private string _categoryName;
        private CategoryColor _categoryColor;
        
        public ICommand SaveSettingsCommand { get; private set; }

        public TagSettingsViewModel(IPersistenceHashTagService storage, ICategoryManagerService categoryManager)
        {
            _storage = storage;
            _categoryManager = categoryManager;
            SaveSettingsCommand = new RelayCommand(SaveSettings);
            
            var tagSettingControl = new HashTagSettings
            {
                DataContext = this
            };
            _popupForm = new Form1(tagSettingControl);
        }
        
        public void ShowSettings(HashTagModel tag)
        {
            HashTag = tag;
            _popupForm.Show();
        }

        private void SetDefaultValues(HashTagModel tag)
        {
            FolderName = string.IsNullOrWhiteSpace(tag.FolderName) ? tag.Id : tag.FolderName;
            CategoryName = string.IsNullOrWhiteSpace(tag.CategoryName) ? tag.Id : tag.CategoryName;
            CategoryColor = tag.CategoryColor == CategoryColor.olCategoryColorNone ? CategoryColor.olCategoryColorDarkBlue : tag.CategoryColor;
            CreateFolderEnabled = tag.CreateFolder;
            CreateCategoryEnabled = tag.CreateCategory;
        }

        public List<CategoryColor> CategoryColorOptions { get; } =
            Enum.GetValues(typeof(CategoryColor)).Cast<CategoryColor>().ToList();
        
        public HashTagModel HashTag
        {
            get => _hashTag;
            set
            {
                SetField(ref _hashTag, value);
                SetDefaultValues(value);
            }
        }

        public bool CreateFolderEnabled
        {
            get => _createFolderEnabled;
            set => SetField(ref _createFolderEnabled, value);
        }

        public bool CreateCategoryEnabled
        {
            get => _createCategoryEnabled;
            set => SetField(ref _createCategoryEnabled, value);
        }

        public string FolderName
        {
            get => _folderName;
            set => SetField(ref _folderName, value);
        }

        public string CategoryName
        {
            get => _categoryName;
            set => SetField(ref _categoryName, value);
        }

        public CategoryColor CategoryColor
        {
            get => _categoryColor;
            set => SetField(ref _categoryColor, value);
        }


        private void SaveSettings()
        {
            _hashTag.CreateFolder = CreateFolderEnabled;
            _hashTag.CreateCategory = CreateCategoryEnabled;
            _hashTag.FolderName = FolderName;
            _hashTag.CategoryName = CategoryName;
            _hashTag.CategoryColor = CategoryColor;
            
            _popupForm.Close();
            
            _storage.SaveHashTag(_hashTag);
            _categoryManager.AssignHashTagItems(_hashTag);
        }
    }

}