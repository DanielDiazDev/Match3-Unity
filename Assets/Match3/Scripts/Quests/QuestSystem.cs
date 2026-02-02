using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;
using UnityEngine;

namespace Match3.Scripts.Quests
{
    public class QuestSystem
    {
        private List<QuestProgress> _activeQuests = new();
        
        public event Action<PieceSO> OnPieceDestroyed;
        public event Action<int> OnScoreUpdated;
        public event Action OnAllObjectivesReached;

        public List<QuestProgress> ActiveQuests => _activeQuests;

        public void Initialize(List<QuestObjectiveSO> objectives)
        {
            _activeQuests.Clear();
            foreach (var progress in objectives.Select(obj => obj.CreateProgress()))
            {
                progress.Initialize(this);
                _activeQuests.Add(progress);
                
                progress.OnProgressChanged += CheckAllObjectives;
            }
        }

        public void NotifyPieceDestroyed(PieceSO piece)
        {
            OnPieceDestroyed?.Invoke(piece);
        }

        public void NotifyScoreUpdated(int score)
        {
            OnScoreUpdated?.Invoke(score);
        }

        private void CheckAllObjectives()
        {
            if (_activeQuests.Any(quest => !quest.IsReached))
            {
                return;
            }
            
            Debug.Log("[QuestSystem] All objectives reached!");
            OnAllObjectivesReached?.Invoke();
        }
    }
}
