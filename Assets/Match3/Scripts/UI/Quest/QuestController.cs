using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Quests;
using Match3.Scripts.Quest;
using Match3.Scripts.Quests;

namespace Match3.Scripts.UI.Quest
{
    public class QuestController : IDisposable
    {
        private readonly QuestSystem _questSystem;
        private readonly QuestViewModel _viewModel;

        public QuestController(QuestSystem questSystem, QuestViewModel viewModel)
        {
            _questSystem = questSystem;
            _viewModel = viewModel;

            Bind();
        }

        private void Bind()
        {
            _questSystem.OnQuestsInitialized += HandleQuestsInitialized;
            _questSystem.OnAllObjectivesReached += HandleAllObjectivesReached;
        }

        private void HandleQuestsInitialized(List<QuestProgress> progresses)
        {
            foreach (var vm in progresses.Select(CreateObjectiveVM))
            {
                _viewModel.Objectives.Add(vm);
            }
        }

        private QuestObjectiveViewModel CreateObjectiveVM(QuestProgress progress)
        {
            var vm = new QuestObjectiveViewModel();

            vm.Icon.Value = progress.Objective.icon;
            vm.ProgressText.Value = progress.GetProgressFormatted();
            vm.IsCompleted.Value = progress.IsReached;

            progress.OnProgressChanged += () =>
            {
                vm.ProgressText.Value = progress.GetProgressFormatted();
                vm.IsCompleted.Value = progress.IsReached;
            };

            return vm;
        }

        private void HandleAllObjectivesReached()
        {
            _viewModel.AllObjectivesCompleted.Execute();
        }

        public void Dispose()
        {
            _questSystem.OnQuestsInitialized -= HandleQuestsInitialized;
            _questSystem.OnAllObjectivesReached -= HandleAllObjectivesReached;
        }
    }
}