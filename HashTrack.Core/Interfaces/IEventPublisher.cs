using System;
using System.Threading.Tasks;

namespace HashTrack.Core.Interfaces
{
    public interface IEventPublisher
    {
        void Subscribe(string eventTag, Action callback);
        void Subscribe(string eventTag, Func<Task> callback);
        void Unsubscribe(string eventTag, Action callback);
        void Unsubscribe(string eventTag, Func<Task> callback);
        //TODO: Could be FireEvent<T> and SUbscribe<T> to pass data
        void FireEvent(string eventTag);
        Task FireEventAsync(string eventTag);
    }

}