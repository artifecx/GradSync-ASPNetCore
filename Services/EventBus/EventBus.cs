using System;
using System.Collections.Concurrent;

namespace Services.EventBus
{
    public class EventBus : IEventBus
    {
        private readonly ConcurrentDictionary<Type, Action<object>> _handlers = new ConcurrentDictionary<Type, Action<object>>();

        public void Publish<T>(T eventItem)
        {
            if (_handlers.TryGetValue(typeof(T), out var handler))
            {
                handler(eventItem);
            }
        }

        public void Subscribe<T>(Action<T> handler)
        {
            _handlers[typeof(T)] = e => handler((T)e);
        }
    }
}
