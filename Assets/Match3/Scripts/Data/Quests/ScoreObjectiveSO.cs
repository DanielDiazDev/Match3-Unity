using UnityEngine;

namespace Match3.Scripts.Data.Quests
{
    [CreateAssetMenu(fileName = "ScoreObjective", menuName = "Quests/Objectives/Score")]
    public class ScoreObjectiveSO : QuestObjectiveSO
    {
        public int targetScore;

        public override QuestProgress CreateProgress()
        {
            return new ScoreProgress(this);
        }
    }

    public class ScoreProgress : QuestProgress
    {
        private readonly ScoreObjectiveSO _so;
        private int _currentScore;

        public ScoreProgress(ScoreObjectiveSO objective) : base(objective)
        {
            _so = objective;
        }

        public override bool IsReached => _currentScore >= _so.targetScore;

        public override string GetProgressFormatted() => $"{_currentScore}/{_so.targetScore}";

        public override void Initialize(Match3.Scripts.Quests.QuestSystem questSystem)
        {
            questSystem.OnScoreUpdated += HandleScoreUpdated;
        }

        private void HandleScoreUpdated(int score)
        {
            _currentScore = score;
            OnProgressChanged?.Invoke();
        }
    }
}
