namespace Match3.Scripts.Event.Data
{
    public class MultiplierScoreChangedEventData : EventData
    {
        public readonly float Score;
        public MultiplierScoreChangedEventData(EventIds eventId, float score) : base(eventId)
        {
            Score = score;
        }
    }
}