using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;

namespace HashTrack.BusinessLogic.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(IEventAggregator))]
    public class EventAggregator : IEventAggregator
    {
        private readonly Dictionary<string, EventHandlers> _eventHandlers = new Dictionary<string, EventHandlers>();

        public void Subscribe(string eventTag, Action callback)
        {
            Subscribe(eventTag, handlers: callback);
        }

        public void Subscribe(string eventTag, Func<Task> callback)
        {
            Subscribe(eventTag, asyncHandlers: callback);
        }

        public void Subscribe(string eventTag, Action<object> callback)
        {
            Subscribe(eventTag, handlersWithData: callback);
        }

        public void Subscribe(string eventTag, Func<object, Task> callback)
        {
            Subscribe(eventTag, asyncHandlersWithData: callback);
        }

        public void Unsubscribe(string eventTag, Action callback)
        {
            Unsubscribe(eventTag, handlers: callback);
        }

        public void Unsubscribe(string eventTag, Func<Task> callback)
        {
            Unsubscribe(eventTag, asyncHandlers: callback);
        }

        public void Unsubscribe(string eventTag, Action<object> callback)
        {
            Unsubscribe(eventTag, handlersWithData: callback);
        }

        public void Unsubscribe(string eventTag, Func<object, Task> callback)
        {
            Unsubscribe(eventTag, asyncHandlersWithData: callback);
        }

        public void FireEvent(string eventTag, object eventData = null)
        {
            var eventHandlers = GetHandlersForKey(eventTag);
            if (eventData is null)
                foreach (var handler in eventHandlers.Handlers)
                    handler.Invoke();
            else
                foreach (var handler in eventHandlers.HandlersWithData)
                    handler.Invoke(eventData);
        }

        public Task FireEventAsync(string eventTag, object eventData = null)
        {
            var eventHandlers = GetHandlersForKey(eventTag);
            var tasks = new List<Task>();
            tasks.AddRange(eventData is null
                ? eventHandlers.AsyncHandlers.Select(handler => handler.Invoke())
                : eventHandlers.AsyncHandlersWithData.Select(handler => handler.Invoke(eventData)));

            return Task.WhenAll(tasks);
        }

        private void Unsubscribe(
            string eventTag,
            Action handlers = null,
            Action<object> handlersWithData = null,
            Func<Task> asyncHandlers = null,
            Func<object, Task> asyncHandlersWithData = null)
        {
            var eventHandlers = GetHandlersForKey(eventTag);
            if (handlers != null) eventHandlers.Handlers.Remove(handlers);
            if (asyncHandlers != null) eventHandlers.AsyncHandlers.Remove(asyncHandlers);
            if (handlersWithData != null) eventHandlers.HandlersWithData.Remove(handlersWithData);
            if (asyncHandlersWithData != null) eventHandlers.AsyncHandlersWithData.Remove(asyncHandlersWithData);
            if (!eventHandlers.Handlers.Any()
                && !eventHandlers.AsyncHandlers.Any()
                && !eventHandlers.HandlersWithData.Any()
                && !eventHandlers.AsyncHandlersWithData.Any())
                _eventHandlers.Remove(eventTag);
        }

        private void Subscribe(
            string eventTag,
            Action handlers = null,
            Action<object> handlersWithData = null,
            Func<Task> asyncHandlers = null,
            Func<object, Task> asyncHandlersWithData = null)
        {
            var eventHandlers = GetHandlersForKey(eventTag);
            if (handlers != null) eventHandlers.Handlers.Add(handlers);
            if (asyncHandlers != null) eventHandlers.AsyncHandlers.Add(asyncHandlers);
            if (handlersWithData != null) eventHandlers.HandlersWithData.Add(handlersWithData);
            if (asyncHandlersWithData != null) eventHandlers.AsyncHandlersWithData.Add(asyncHandlersWithData);
        }

        private EventHandlers GetHandlersForKey(string eventTag)
        {
            if (!_eventHandlers.ContainsKey(eventTag)) _eventHandlers[eventTag] = new EventHandlers();
            return _eventHandlers[eventTag];
        }

        private class EventHandlers
        {
            public EventHandlers()
            {
                Handlers = new List<Action>();
                AsyncHandlers = new List<Func<Task>>();
                HandlersWithData = new List<Action<object>>();
                AsyncHandlersWithData = new List<Func<object, Task>>();
            }

            public List<Action> Handlers { get; }
            public List<Func<Task>> AsyncHandlers { get; }
            public List<Action<object>> HandlersWithData { get; }
            public List<Func<object, Task>> AsyncHandlersWithData { get; }
        }
    }
}