using UniRx;
using UnityEngine;

namespace Match3.Scripts.Quest
{
    public class QuestViewModel
    {
        public readonly ReactiveCollection<QuestObjectiveViewModel> Objectives 
            = new ReactiveCollection<QuestObjectiveViewModel>();

        public readonly ReactiveCommand AllObjectivesCompleted = new();
    }
    public class QuestObjectiveViewModel
    {
        public readonly ReactiveProperty<Sprite> Icon = new();
        public readonly ReactiveProperty<string> ProgressText = new();
        public readonly ReactiveProperty<bool> IsCompleted = new();
    }
}