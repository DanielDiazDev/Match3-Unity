using System;
using Match3.Scripts.Score;
using TMPro;
using UniRx;
using UnityEngine;

namespace Match3.Scripts.UI.Score
{
    public class ScoreView : ViewBase
    {
        [SerializeField] private TextMeshProUGUI _scoreText;
       // [SerializeField] private TextMeshProUGUI _multiplierText;
        [SerializeField] private ScoreSystem _scoreSystem;
        private ScoreViewModel _viewModel;
        private ScoreController _controller;

        private void Start()
        {
            var viewModel = new ScoreViewModel();
            _controller = new ScoreController(_scoreSystem, viewModel);
            _disposables.Add(_controller);
            Configure(viewModel);
        }

        public void Configure(ScoreViewModel viewModel)
        {
            _viewModel = viewModel;

            _viewModel.Score
                .Subscribe(score => _scoreText.SetText(score.ToString()))
                .AddTo(_disposables);

            // _viewModel.Multiplier
            //     .Subscribe(multiplier => _multiplierText.SetText($"x{multiplier:0.0}"))
            //     .AddTo(_disposables);
        }

        
    }
}