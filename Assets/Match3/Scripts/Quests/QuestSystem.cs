using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;
using UnityEngine;

namespace Match3.Scripts.Quests
{
    public class QuestSystem : MonoBehaviour
    {
        private List<QuestProgress> _activeQuests = new();
        
        public event Action<PieceSO> OnPieceDestroyed;
        public event Action<int> OnScoreUpdated;
        public event Action OnAllObjectivesReached;
        public event Action OnObjectiveCompleted;
        public event Action<List<QuestProgress>> OnQuestsInitialized;

        public List<QuestProgress> ActiveQuests => _activeQuests;
        public int CompletedQuestsCount => _activeQuests.Count(q => q.IsReached);

        public void Initialize(List<QuestObjectiveSO> objectives)
        {
            foreach (var quest in _activeQuests)
            {
                quest.Dispose();
            }

            _activeQuests.Clear();

            foreach (var progress in objectives.Take(3).Select(obj => obj.CreateProgress()))
            {
                progress.Initialize(this);
                _activeQuests.Add(progress);

                progress.OnProgressChanged += () =>
                {
                    if (progress.IsReached)
                        OnObjectiveCompleted?.Invoke();

                    CheckAllObjectives();
                };
            }

            OnQuestsInitialized?.Invoke(_activeQuests);
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
