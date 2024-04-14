using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using HashTrack.Core.Attributes;
using HashTrack.Core.Enums;
using HashTrack.Core.Interfaces;

namespace HashTrack.BusinessLogic.Services
{
    [RegisterService(LifeCycle.Singleton, typeof(IEventPublisher))]
    public class EventPublisher : IEventPublisher
    {
            private readonly Dictionary<string, List<Action>> _eventHandlers = new Dictionary<string, List<Action>>();
            private readonly Dictionary<string, List<Func<Task>>> _asyncEventHandlers = new Dictionary<string, List<Func<Task>>>();

            public void Subscribe(string eventTag, Action callback)
            {
                if (!_eventHandlers.ContainsKey(eventTag))
                {
                    _eventHandlers[eventTag] = new List<Action>();
                }
                _eventHandlers[eventTag].Add(callback);
            }

            public void Subscribe(string eventTag, Func<Task> callback)
            {
                if (!_asyncEventHandlers.ContainsKey(eventTag))
                {
                    _asyncEventHandlers[eventTag] = new List<Func<Task>>();
                }
                _asyncEventHandlers[eventTag].Add(callback);
            }

            public void Unsubscribe(string eventTag, Action callback)
            {
                if (_eventHandlers.ContainsKey(eventTag))
                {
                    _eventHandlers[eventTag].Remove(callback);
                    if (!_eventHandlers[eventTag].Any())
                    {
                        _eventHandlers.Remove(eventTag);
                    }
                }
            }

            public void Unsubscribe(string eventTag, Func<Task> callback)
            {
                if (_asyncEventHandlers.ContainsKey(eventTag))
                {
                    _asyncEventHandlers[eventTag].Remove(callback);
                    if (!_asyncEventHandlers[eventTag].Any())
                    {
                        _asyncEventHandlers.Remove(eventTag);
                    }
                }
            }

            public void FireEvent(string eventTag)
            {
                if (_eventHandlers.ContainsKey(eventTag))
                {
                    foreach (var handler in _eventHandlers[eventTag])
                    {
                        handler.Invoke();
                    }
                }
            }

            public Task FireEventAsync(string eventTag)
            {
                if (_asyncEventHandlers.ContainsKey(eventTag))
                {
                    var tasks = _asyncEventHandlers[eventTag].Select(handler => handler.Invoke());
                    return Task.WhenAll(tasks);
                }
                return Task.CompletedTask;
            }
    }
}