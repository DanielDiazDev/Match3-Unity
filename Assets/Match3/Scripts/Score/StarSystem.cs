using System;
using Match3.Scripts.Data.Level;
using Match3.Scripts.Quests;
using UnityEngine;

namespace Match3.Scripts.Score
{
    public class StarSystem
    {
        private readonly LevelDataSO _levelData;
        private readonly ScoreSystem _scoreSystem;
        private readonly QuestSystem _questSystem;

        public event Action<int> OnStarPowerChanged;
        public event Action<int> OnStarReached;

        public int CurrentStarPower { get; private set; }
        public int CurrentStars { get; private set; }

        public StarSystem(LevelDataSO levelData, ScoreSystem scoreSystem, QuestSystem questSystem)
        {
            _levelData = levelData;
            _scoreSystem = scoreSystem;
            _questSystem = questSystem;

            _scoreSystem.OnScoreChanged += HandleScoreChanged;
            _questSystem.OnObjectiveCompleted += HandleObjectiveCompleted;
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
            int score = _scoreSystem.CurrentScore;
            int completedQuests = _questSystem.CompletedQuestsCount;
            int totalBonus = completedQuests * _levelData.objectiveBonus;

            CurrentStarPower = score + totalBonus;
            
            CheckStars();
            OnStarPowerChanged?.Invoke(CurrentStarPower);
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
                OnStarReached?.Invoke(CurrentStars);
            }
        }

        public void ForceMinimumStar()
        {
            // If all objectives are reached but no stars yet, force 1 star
            if (CurrentStars < 1 && _levelData.starThresholds.Length > 0)
            {
                CurrentStarPower = Mathf.Max(CurrentStarPower, _levelData.starThresholds[0]);
                CurrentStars = 1;
                OnStarPowerChanged?.Invoke(CurrentStarPower);
                OnStarReached?.Invoke(CurrentStars);
            }
        }
    }
}
