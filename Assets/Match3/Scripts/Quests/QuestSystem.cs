using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Item;
using Match3.Scripts.Data.Quests;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using UnityEngine;

namespace Match3.Scripts.Quests
{
    public class QuestSystem : IQuestSystem, IEventObserver, IDisposable
    {
        private List<QuestProgress> _activeQuests = new();
        
        public event Action<PieceSO> OnPieceDestroyed;
        // public event Action OnAllObjectivesReached;
        // public event Action<List<QuestProgress>> OnQuestsInitialized;

        public List<QuestProgress> ActiveQuests => _activeQuests;
        public int CompletedQuestsCount => _activeQuests.Count(q => q.IsReached);


        public void Process(EventData eventData)
        {
            
        }
        public QuestSystem(List<QuestObjectiveSO> objectives)
        {
            EventQueue.Instance.Subscribe(EventIds.ScoreChanged, this);
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
                        EventQueue.Instance.EnqueueEvent(new EventData(EventIds.ObjectiveCompleted));

                    CheckAllObjectives();
                };
            }
            var questsInitializedEventData = new QuestsInitializedEventData(EventIds.QuestsInitialized, _activeQuests);
            EventQueue.Instance.EnqueueEvent(questsInitializedEventData);
        }



        public void NotifyPieceDestroyed(PieceSO piece)
        {
            OnPieceDestroyed?.Invoke(piece);
        }

        // public void NotifyScoreUpdated(int score)
        // {
        //     OnScoreUpdated?.Invoke(score);
        // }

        private void CheckAllObjectives()
        {
            if (_activeQuests.Any(quest => !quest.IsReached))
            {
                return;
            }
            
            Debug.Log("[QuestSystem] All objectives reached!");
            EventQueue.Instance.EnqueueEvent(new EventData(EventIds.AllObjectivesReached));
        }


        public void Dispose()
        {
            EventQueue.Instance.Unsubscribe(EventIds.ScoreChanged, this);
        }
    }
}
