using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    public class PersistanceService
    {
        private Outlook.Application _application { get; set; }
        public PersistanceService(Outlook.Application application)
        {
            _application = application;
        }

        public void Save(string storageId, string metadata)
        {
            Outlook.StorageItem storageItem = GetStorage(storageId);

            storageItem.Body = metadata;
            storageItem.Save();
        }
        
        public string Load(string storageId)
        {
            return GetStorage(storageId).Body;
        }

        private Outlook.StorageItem GetStorage(string id)
        {
            Outlook.Folder folder = _application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox) as Outlook.Folder;
            return folder.GetStorage(id, Outlook.OlStorageIdentifierType.olIdentifyBySubject);
        }
        
    }
}