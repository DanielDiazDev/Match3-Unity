namespace Match3.Scripts.Event.Data
{
    public class MovesChangedEventData : EventData
    {
        public readonly int Moves;

        public MovesChangedEventData(EventIds eventId, int moves) : base(eventId)
        {
            Moves = moves;
        }
    }
}