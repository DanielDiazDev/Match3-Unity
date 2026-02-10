using System;
using System.Collections.Generic;
using System.Linq;
using Match3.Scripts.Data.Quests;
using Match3.Scripts.Event;
using Match3.Scripts.Event.Data;
using Match3.Scripts.Quest;
using Match3.Scripts.Quests;

namespace Match3.Scripts.UI.Quest
{
    public class QuestController : IDisposable, IEventObserver
    {
        private readonly QuestViewModel _viewModel;
        private bool isInit;

        public QuestController(QuestViewModel viewModel)
        {
            _viewModel = viewModel;
            EventQueue.Instance.Subscribe(EventIds.QuestsInitialized, this);
            EventQueue.Instance.Subscribe(EventIds.AllObjectivesReached, this);

        }
        public void Process(EventData eventData)
        {
            if (eventData.EventId == EventIds.QuestsInitialized)
            {
                isInit = true;
                var questsInitializedEventData = (QuestsInitializedEventData)eventData;
                HandleQuestsInitialized(questsInitializedEventData.questProgress);
            }

            if (eventData.EventId == EventIds.AllObjectivesReached)
            {
                HandleAllObjectivesReached();
            }
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
            EventQueue.Instance.Unsubscribe(EventIds.QuestsInitialized, this);
            EventQueue.Instance.Unsubscribe(EventIds.AllObjectivesReached, this);
        }

        
    }
}