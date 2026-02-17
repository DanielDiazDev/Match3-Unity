namespace Match3.Scripts.Event.Data
{
    public class StarReachedEventData : EventData
    {
        public readonly int CurrentStar;

        public StarReachedEventData(EventIds eventId, int currentStar, int priority = 0) 
            : base(eventId, priority)
        {
            CurrentStar = currentStar;
        }
    }
}