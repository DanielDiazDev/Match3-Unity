namespace Match3.Scripts.Event.Data
{
    public class EventData
    {
        public readonly EventIds EventId;
        public readonly int Priority;

        public EventData(EventIds eventId, int priority = 0)
        {
            EventId = eventId;
            Priority = priority;
        }
    }
}