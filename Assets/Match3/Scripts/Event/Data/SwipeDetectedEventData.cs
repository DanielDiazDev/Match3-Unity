using UnityEngine;

namespace Match3.Scripts.Event.Data
{
    public class SwipeDetectedEventData : EventData
    {
        public readonly Vector2 worldStart;
        public readonly Vector2Int direction;

        public SwipeDetectedEventData(EventIds eventId, Vector2 worldStart, Vector2Int direction) : base(eventId)
        {
            this.worldStart = worldStart;
            this.direction = direction;
        }
    }
}