namespace Match3.Scripts.Event.Data
{
    public class StarPowerInitializedEventData : EventData
    {
        public readonly int[] StarThreshold;

        public StarPowerInitializedEventData(EventIds eventId, int[] starThreshold, int priority = 0) 
            : base(eventId, priority)
        {
            StarThreshold = starThreshold;
        }
    }
}