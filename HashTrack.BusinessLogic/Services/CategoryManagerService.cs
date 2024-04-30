using HashTrack.Core;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;
using HashTrack.Core.Interfaces.Search;
using HashTrack.Core.Models.Search;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.BusinessLogic.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(ICategoryManagerService))]
    public class CategoryManagerService : ICategoryManagerService
    {
        private readonly Outlook.Application _application;
        private readonly IEventAggregator _eventAggregator;
        private readonly ISearchService _searchService;

        public CategoryManagerService(ISearchService searchService, Outlook.Application application,
            IEventAggregator eventAggregator)
        {
            _application = application;
            _eventAggregator = eventAggregator;
            _searchService = searchService;
        }

        public void AssignHashTagItems(HashTagModel hashTagModel)
        {
            //TODO: Create new eventHandler for this; would need to hack this to await in synchronous method
            _searchService.SearchAllItemsForTag(hashTagModel);
            _eventAggregator.Unsubscribe(Events.CategoryManagerSearch, HandleSearchComplete);
            _eventAggregator.Subscribe(Events.CategoryManagerSearch, HandleSearchComplete);


            void HandleSearchComplete(object obj)
            {
                if (!(obj is Outlook.Search searchResult)) return;

                foreach (var item in searchResult.Results) AddItemToCategory(hashTagModel, item);
            }
        }

        public void AddItemToCategory(HashTagModel hashTagModel, object item)
        {
            var categoryName = string.IsNullOrWhiteSpace(hashTagModel.CategoryName)
                ? hashTagModel.Id
                : hashTagModel.CategoryName;
            var categoryColor = hashTagModel.CategoryColor;
            var session = _application.Session;

            var category = session.Categories[categoryName];
            if (category == null)
                category = session.Categories.Add(categoryName, (Outlook.OlCategoryColor)(int)categoryColor);

            AssignCategoryToItem(item, category);
        }


        private void AssignCategoryToItem(object item, Outlook.Category category)
        {
            switch (item)
            {
                case Outlook.MailItem mailItem:
                    if (string.IsNullOrWhiteSpace(mailItem.Categories) || !mailItem.Categories.Contains(category.Name))
                    {
                        mailItem.Categories += category.Name;
                        mailItem.Save();
                    }

                    break;
                case Outlook.ContactItem contactItem:
                    if (string.IsNullOrWhiteSpace(contactItem.Categories) ||
                        !contactItem.Categories.Contains(category.Name))
                    {
                        contactItem.Categories += category.Name;
                        contactItem.Save();
                    }

                    break;
                case Outlook.AppointmentItem appointmentItem:
                    if (string.IsNullOrWhiteSpace(appointmentItem.Categories) ||
                        !appointmentItem.Categories.Contains(category.Name))
                    {
                        appointmentItem.Categories += category.Name;
                        appointmentItem.Save();
                    }

                    break;
                case Outlook.TaskItem taskItem:
                    if (string.IsNullOrWhiteSpace(taskItem.Categories) || !taskItem.Categories.Contains(category.Name))
                    {
                        taskItem.Categories += category.Name;
                        taskItem.Save();
                    }

                    break;
            }
        }
    }
}