using System;
using System.Threading.Tasks;

namespace HashTrack.Core.Interfaces
{
    public interface IEventAggregator
    {
        // Subscribe to events without data
        void Subscribe(string eventTag, Action callback);
        void Subscribe(string eventTag, Func<Task> callback);

        // Subscribe to events with data
        void Subscribe(string eventTag, Action<object> callback);
        void Subscribe(string eventTag, Func<object, Task> callback);

        // Unsubscribe from events without data
        void Unsubscribe(string eventTag, Action callback);
        void Unsubscribe(string eventTag, Func<Task> callback);

        // Unsubscribe from events with data
        void Unsubscribe(string eventTag, Action<object> callback);
        void Unsubscribe(string eventTag, Func<object, Task> callback);

        // Fire events with data
        void FireEvent(string eventTag, object eventData = null);
        Task FireEventAsync(string eventTag, object eventData = null);
    }
}