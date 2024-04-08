namespace HashTrack.Persistance.Interfaces
{
    public interface IStorage
    {
        void Set<T>(string id, T value);
        bool TryGet<T>(string id, out T value);
        void Remove<T>(string id);
    }
}