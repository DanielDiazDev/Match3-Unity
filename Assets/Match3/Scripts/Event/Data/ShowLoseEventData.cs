namespace Match3.Scripts.Event.Data
{
    public class ShowLoseEventData : EventData
    {
        public readonly int Score;

        public ShowLoseEventData(EventIds eventId, int score) : base(eventId)
        {
            Score = score;
        }
    }
}