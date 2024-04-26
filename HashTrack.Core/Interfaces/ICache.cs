namespace HashTrack.Core.Interfaces
{
    public interface ICache
    {
        bool TryGet<T>(string key, out T item);
        void Set<T>(string key, T value);
        void Remove(string key);
    }
}