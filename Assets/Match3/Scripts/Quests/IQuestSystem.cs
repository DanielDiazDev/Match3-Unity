using System.Collections.Generic;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;

namespace Match3.Scripts.Quests
{
    public interface IQuestSystem
    {
        void NotifyPieceDestroyed(PieceSO piece);
        int CompletedQuestsCount { get; }
        List<QuestProgress> ActiveQuests { get; }
    }
}