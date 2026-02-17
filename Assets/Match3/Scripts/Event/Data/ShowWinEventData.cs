namespace Match3.Scripts.Event.Data
{
    public class ShowWinEventData : EventData
    {
        public readonly int Score;

        public ShowWinEventData(EventIds eventId, int score) : base(eventId)
        {
            Score = score;
        }
    }
}