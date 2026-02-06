using System;
using System.Collections.Generic;
using Match3.Scripts.Score;

namespace Match3.Scripts.UI.Score
{
    public class ScoreController : IDisposable
    {
        private readonly ScoreSystem _scoreSystem;
        private readonly ScoreViewModel _viewModel;
        private readonly List<IDisposable> _disposables = new();

        public ScoreController(ScoreSystem scoreSystem, ScoreViewModel viewModel)
        {
            _scoreSystem = scoreSystem;
            _viewModel = viewModel;

            Bind();
            Initialize();
        }

        private void Initialize()
        {
            _viewModel.Score.Value = _scoreSystem.CurrentScore;
            _viewModel.Multiplier.Value = _scoreSystem.CurrentMultiplier;
        }

        private void Bind()
        {
            _scoreSystem.OnScoreChanged += OnScoreChanged;
            _scoreSystem.OnMultiplierChanged += OnMultiplierChanged;
        }

        private void OnScoreChanged(int score)
        {
            _viewModel.Score.Value = score;
        }

        private void OnMultiplierChanged(float multiplier)
        {
            _viewModel.Multiplier.Value = multiplier;
        }

        public void Dispose()
        {
            _scoreSystem.OnScoreChanged -= OnScoreChanged;
            _scoreSystem.OnMultiplierChanged -= OnMultiplierChanged;
        }
    }
}