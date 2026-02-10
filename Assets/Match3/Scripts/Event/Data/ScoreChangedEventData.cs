namespace Match3.Scripts.Event.Data
{
    public class ScoreChangedEventData : EventData
    {
        public readonly int Score;

        public ScoreChangedEventData(EventIds eventId, int score) : base(eventId)
        {
            Score = score;
        }
    }
}