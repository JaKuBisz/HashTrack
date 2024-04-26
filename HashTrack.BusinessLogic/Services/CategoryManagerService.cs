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
        private readonly IArtifactSearchService _artifactSearchService;
        private readonly Outlook.Application _application;
        private readonly IEventAggregator _eventAggregator;

        public CategoryManagerService(IArtifactSearchService artifactSearchService, Outlook.Application application, IEventAggregator eventAggregator)
        {
            _application = application;
            _eventAggregator = eventAggregator;
            _artifactSearchService = artifactSearchService;
        }
        
        public void AssignHashTagItems(HashTagModel hashTagModel)
        {
            //TODO: Create new eventHandler for this; would need to hack this to await in synchronous method
            _artifactSearchService.SearchAllItemsForTag(hashTagModel);
            _eventAggregator.Subscribe(Events.CategoryManagerSearch, HandleSearchComplete);
            
            

            void HandleSearchComplete(object obj)
            {
                if (!(obj is Outlook.Search searchResult))
                {
                    return;
                }
            
                foreach (object item in searchResult.Results)
                {
                    AddItemToCategory(hashTagModel.CategoryName, hashTagModel.CategoryColor, item);
                }
                
                _eventAggregator.Unsubscribe(Events.CategoryManagerSearch, HandleSearchComplete);
            }
        }
        
        public void AddItemToCategory(string categoryName, CategoryColor categoryColor, object item)
        {
            Outlook.NameSpace session = _application.Session;

            // Check if the category already exists
            Outlook.Category category = session.Categories[categoryName];
            if (category == null)
            {
                category = session.Categories.Add(categoryName, ((Outlook.OlCategoryColor)(int)categoryColor));
            }

            // Assign the category to a mail item
            AssignCategoryToItem(item, category);
        }
        

        private void AssignCategoryToItem(object item, Outlook.Category category)
        {
            switch (item)
            {
                case Outlook.MailItem mailItem:
                    if (!mailItem.Categories.Contains(category.Name))
                    {
                        mailItem.Categories += category.Name;
                        mailItem.Save();
                    }
                    break;
                case Outlook.ContactItem contactItem:
                    if(!contactItem.Categories.Contains(category.Name))
                    {
                        contactItem.Categories += category.Name;
                        contactItem.Save();
                    }
                    break;
                case Outlook.AppointmentItem appointmentItem:
                    if(!appointmentItem.Categories.Contains(category.Name))
                    {
                        appointmentItem.Categories += category.Name;
                        appointmentItem.Save();
                    }
                    break;
                case Outlook.TaskItem taskItem:
                    if(!taskItem.Categories.Contains(category.Name))
                    {
                        taskItem.Categories += category.Name;
                        taskItem.Save();
                    }
                    break;
            }
        }
        
    }
}