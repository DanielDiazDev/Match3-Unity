using System.Collections.Generic;
using Match3.Scripts.Data.Quests;

namespace Match3.Scripts.Event.Data
{
    public class QuestsInitializedEventData : EventData
    {
        public readonly List<QuestProgress>  questProgress;

        public QuestsInitializedEventData(EventIds eventId, List<QuestProgress> questProgress) : base(eventId)
        {
            this.questProgress = questProgress;
        }
    }
}