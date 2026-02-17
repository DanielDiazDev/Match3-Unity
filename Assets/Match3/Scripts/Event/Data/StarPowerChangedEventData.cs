namespace Match3.Scripts.Event.Data
{
    public class StarPowerChangedEventData : EventData
    {
        public readonly int CurrentStarPower;

        public StarPowerChangedEventData(EventIds eventId, int currentStarPower, int priority = 0) 
            : base(eventId, priority)
        {
            CurrentStarPower = currentStarPower;
        }
    }
}