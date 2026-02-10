using System.Collections.Generic;
using Match3.Scripts.Event.Data;
using UnityEngine;

namespace Match3.Scripts.Event
{
    public class EventQueue : MonoBehaviour
    {
        public static EventQueue Instance { get; private set; }
        private Queue<EventData> _currentEvents = new Queue<EventData>();
        private Queue<EventData> _nextEvents = new Queue<EventData>();
        private Dictionary<EventIds, List<IEventObserver>> _observers = new Dictionary<EventIds, List<IEventObserver>>();

        private void Awake()
        {
            Instance = this;
        }

        public void Subscribe(EventIds eventId, IEventObserver observer)
        {
            if (!_observers.TryGetValue(eventId, out var eventObservers))
            {
                eventObservers = new List<IEventObserver>();
            }
            eventObservers.Add(observer);
            _observers[eventId] = eventObservers;
        }

        public void Unsubscribe(EventIds eventId, IEventObserver observer)
        {
            _observers[eventId].Remove(observer);
        }
        public void EnqueueEvent(EventData eventData)
        {
            _nextEvents.Enqueue(eventData);
            Debug.Log($"Enqueued event {eventData.EventId} on frame {Time.frameCount}");
        }

        private void LateUpdate()
        {
            ProcessEvents();
        }

        private void ProcessEvents()
        {
            (_currentEvents, _nextEvents) = (_nextEvents, _currentEvents);
            foreach (var currentEvent in _currentEvents)
            {
                ProcessEvent(currentEvent);
            }
            _currentEvents.Clear();
            //Si queremos procesar todos los eventos en el mismo frame
            /*if (_nextEvents.Count > 0)
            {
                ProcessEvents();
            }*/
        }

        private void ProcessEvent(EventData eventData)
        {
            Debug.Log($"Processing event {eventData.EventId} on frame {Time.frameCount}");
            if (_observers.TryGetValue(eventData.EventId, out var eventObservers))
            {
                foreach (var eventObserver in eventObservers)
                {
                    eventObserver.Process(eventData);
                }
            }
        }
    }
}