using System;
using Match3.Scripts.Quests;
using Match3.Scripts.UI.Quest;
using UniRx;
using UnityEngine;

namespace Match3.Scripts.Quest
{
    public class QuestView : ViewBase
    {
        [SerializeField] private QuestObjectiveView _objectivePrefab;
        [SerializeField] private Transform _container;
        [SerializeField] private QuestSystem _questSystem;
        private QuestController _questController;
        private void Start()
        {
            var viewModel = new QuestViewModel();
            _questController = new QuestController(_questSystem, viewModel);
            _disposables.Add(_questController);
            Configure(viewModel);
        }

        public void Configure(QuestViewModel viewModel)
        {
            viewModel.Objectives.ObserveAdd()
                .Subscribe(addEvent =>
                {
                    var view = Instantiate(_objectivePrefab, _container);
                    view.Configure(addEvent.Value);
                })
                .AddTo(_disposables);
        }
    }
}