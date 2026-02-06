using UnityEngine;

namespace Match3.Scripts.Data.Quests
{
    public abstract class QuestObjectiveSO : ScriptableObject
    {
        public string objectiveDescription;
        public Sprite icon;
        
        public abstract QuestProgress CreateProgress();
    }

    public abstract class QuestProgress
    {
        public QuestObjectiveSO Objective { get; }
        public abstract bool IsReached { get; }
        public abstract string GetProgressFormatted();
        
        public System.Action OnProgressChanged;

        protected QuestProgress(QuestObjectiveSO objective)
        {
            Objective = objective;
        }

        public abstract void Initialize(Match3.Scripts.Quests.QuestSystem questSystem);
        public abstract void Dispose();
    }
}
