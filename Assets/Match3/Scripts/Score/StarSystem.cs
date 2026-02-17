using System;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Quests;
using UnityEngine;

namespace Match3.Scripts.Score
{
    public class StarSystem : IDisposable, IEventObserver
    {
        private readonly LevelDataSO _levelData;
        private IQuestSystem _questSystem;
        private IScoreSystem _scoreSystem;

        public int CurrentStarPower { get; private set; }
        public int CurrentStars { get; private set; }

        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.ObjectiveCompleted)
            {
                HandleObjectiveCompleted();
            }

            if (eventData.EventId == EventIds.ScoreChanged)
            {
                var scoreChangedEventData = (ScoreChangedEventData)eventData;
                HandleScoreChanged(scoreChangedEventData.Score);
            }
        }
        public StarSystem(LevelDataSO levelData, IQuestSystem questSystem, IScoreSystem scoreSystem)
        {
            _levelData = levelData;
            _questSystem = questSystem;
            _scoreSystem = scoreSystem;
            EventQueue.Instance.Subscribe(EventIds.ObjectiveCompleted, this);
            EventQueue.Instance.Subscribe(EventIds.ScoreChanged, this);
            var starPowerInitializedEventData = new StarPowerInitializedEventData(EventIds.StarsInitialized, _levelData.starThresholds);
            EventQueue.Instance.EnqueueEvent(starPowerInitializedEventData);
            
        }

        private void HandleScoreChanged(int score)
        {
           
            UpdateStarPower();
        }

        private void HandleObjectiveCompleted()
        {
            UpdateStarPower();
        }

        private void UpdateStarPower()
        {
            var score = _scoreSystem.CurrentScore;
            var completedQuests = _questSystem.CompletedQuestsCount;
            var totalBonus = completedQuests * _levelData.objectiveBonus;

            CurrentStarPower = score + totalBonus;
            
            CheckStars();
            var starPowerChangedEventData =
                new StarPowerChangedEventData(EventIds.StarPowerChanged, CurrentStarPower);
            EventQueue.Instance.EnqueueEvent(starPowerChangedEventData);
        }

        private void CheckStars()
        {
            int starsReached = 0;
            for (int i = 0; i < _levelData.starThresholds.Length; i++)
            {
                if (CurrentStarPower >= _levelData.starThresholds[i])
                {
                    starsReached = i + 1;
                }
                else
                {
                    break;
                }
            }

            if (starsReached > CurrentStars)
            {
                CurrentStars = starsReached;
                var starReachedEventData =
                    new StarReachedEventData(EventIds.StarReachedChanged, CurrentStars, 1);
                EventQueue.Instance.EnqueueEvent(starReachedEventData);
            }
        }

        public void ForceMinimumStar()
        {
            // If all objectives are reached but no stars yet, force 1 star
            if (CurrentStars < 1 && _levelData.starThresholds.Length > 0)
            {
                CurrentStarPower = Mathf.Max(CurrentStarPower, _levelData.starThresholds[0]);
                CurrentStars = 1;
                var starPowerChangedEventData =
                    new StarPowerChangedEventData(EventIds.StarPowerChanged, CurrentStarPower);
                EventQueue.Instance.EnqueueEvent(starPowerChangedEventData);
                var starReachedEventData =
                    new StarReachedEventData(EventIds.StarReachedChanged, CurrentStars, 1);
                EventQueue.Instance.EnqueueEvent(starReachedEventData);
            }
        }

        public void Dispose() // TODO Alguien lo tiene que llamar
        {
            EventQueue.Instance.Unsubscribe(EventIds.ObjectiveCompleted, this);
            EventQueue.Instance.Unsubscribe(EventIds.ScoreChanged, this);
        }

        
    }
}
