using System.Collections.Generic;
using Match3.Scripts.Event.Data;
using UnityEngine;

namespace Match3.Scripts.Event
{
    public class EventQueue : MonoBehaviour
    {
        public static EventQueue Instance { get; private set; }
        // Frame actual
        private Dictionary<int, Queue<EventData>> _currentEvents = new();
        // Siguiente frame
        private Dictionary<int, Queue<EventData>> _nextEvents = new();

        private readonly Dictionary<EventIds, List<IEventObserver>> _observers = new();

        private void Awake()
        {
            Instance = this;
        }

        public void Subscribe(EventIds eventId, IEventObserver observer)
        {
            if (!_observers.TryGetValue(eventId, out var eventObservers))
            {
                eventObservers = new List<IEventObserver>();
                _observers[eventId] = eventObservers;
            }

            eventObservers.Add(observer);
        }

        public void Unsubscribe(EventIds eventId, IEventObserver observer)
        {
            if (_observers.TryGetValue(eventId, out var eventObservers))
            {
                eventObservers.Remove(observer);
            }
        }

        public void EnqueueEvent(EventData eventData)
        {
            if (!_nextEvents.TryGetValue(eventData.Priority, out var queue))
            {
                queue = new Queue<EventData>();
                _nextEvents[eventData.Priority] = queue;
            }

            queue.Enqueue(eventData);
        }

        private void LateUpdate()
        {
            ProcessEvents();
        }

        private void ProcessEvents()
        {
            // Swap buffers
            (_currentEvents, _nextEvents) = (_nextEvents, _currentEvents);

            if (_currentEvents.Count == 0)
                return;

            // Procesar de mayor prioridad a menor
            var priorities = new List<int>(_currentEvents.Keys);
            priorities.Sort((a, b) => b.CompareTo(a)); // DESC

            foreach (var priority in priorities)
            {
                var queue = _currentEvents[priority];

                while (queue.Count > 0)
                {
                    var eventData = queue.Dequeue();
                    ProcessEvent(eventData);
                }
            }

            _currentEvents.Clear();
        }

        private void ProcessEvent(EventData eventData)
        {
            if (_observers.TryGetValue(eventData.EventId, out var observers))
            {
                foreach (var observer in observers)
                {
                    observer.Process(eventData);
                }
            }
        }
    }
}