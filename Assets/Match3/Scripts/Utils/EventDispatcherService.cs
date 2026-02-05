using System;
using System.Collections.Generic;

namespace Match3.Scripts.Utils
{
    public class EventDispatcherService
    {
        public static EventDispatcherService Instance => _instance ??= new EventDispatcherService();
        private static EventDispatcherService _instance;

        private readonly Dictionary<Type, Delegate> _events;

        private EventDispatcherService()
        {
            _events = new Dictionary<Type, Delegate>();
        }

        public void Subscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                _events[type] = Delegate.Combine(_events[type], callback);
            }
            else
            {
                _events[type] = callback;
            }
        }

        public void Unsubscribe<T>(Action<T> callback)
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                _events[type] = Delegate.Remove(_events[type], callback);
            }
        }

        public void Dispatch<T>(T signal)
        {
            var type = typeof(T);
            if (_events.ContainsKey(type))
            {
                var action = _events[type] as Action<T>;
                action?.Invoke(signal);
            }
        }
    }
}