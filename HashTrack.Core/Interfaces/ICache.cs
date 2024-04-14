namespace HashTrack.Core.Interfaces
{
    public interface ICache<T> where T : class
    {
        T Get(string key);
        void Set(string key, T value);
        void Remove(string key);
    }
}