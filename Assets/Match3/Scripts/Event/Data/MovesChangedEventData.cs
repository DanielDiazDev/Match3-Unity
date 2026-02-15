namespace Match3.Scripts.Event.Data
{
    public class MovesChangedEventData : EventData
    {
        public readonly int Moves;

        public MovesChangedEventData(EventIds eventId, int moves, int priority = 0) 
            : base(eventId, priority)
        {
            Moves = moves;
        }
    }
}