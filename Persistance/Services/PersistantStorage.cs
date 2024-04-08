using System;
using System.IO;
using System.Xml.Serialization;
using HashTrack.Enums;
using HashTrack.Helpers;
using HashTrack.Persistance.Interfaces;
using Newtonsoft.Json;
using Outlook = Microsoft.Office.Interop.Outlook;

namespace HashTrack.Services
{
    [RegisterService(typeof(PersistantStorage), LifeCycle.Singleton)]

    public class PersistantStorage : IStorage
    {
        private Outlook.Application _application { get; set; }
        public PersistantStorage(Outlook.Application application)
        {
            _application = application;
        }

        public void Set<T>(string id, T value)
        {
            Outlook.StorageItem storageItem = GetStorage(id);
            
            var serializedValue = JsonConvert.SerializeObject(value);
            storageItem.Body = serializedValue;
            storageItem.Save();
        }

        public bool TryGet<T>(string id, out T value)
        {
            var serializedValue = GetStorage(id).Body;
            if (serializedValue == null)
            {
                value = default;
                return false;
            }

            value = JsonConvert.DeserializeObject<T>(serializedValue);

            return value != null;
        }

        public void Remove<T>(string id)
        {
            GetStorage(id).Delete();
        }

        private Outlook.StorageItem GetStorage(string id)
        {
            Outlook.Folder folder = _application.Session.GetDefaultFolder(Outlook.OlDefaultFolders.olFolderInbox) as Outlook.Folder;
            return folder.GetStorage(id, Outlook.OlStorageIdentifierType.olIdentifyBySubject);
        }
    }
}