namespace Match3.Scripts.Event.Data
{
    public class ShowWinEventData : EventData
    {
        public readonly int Score;
        public readonly int Star;

        public ShowWinEventData(EventIds eventId, int score, int star) : base(eventId)
        {
            Score = score;
            Star = star;
        }
    }
}