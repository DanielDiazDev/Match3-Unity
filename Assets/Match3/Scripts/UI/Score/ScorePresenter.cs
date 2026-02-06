using System;
using Match3.Scripts.UI.Score;

namespace UI.Score.Score
{
    public class ScorePresenter : IDisposable
    {
        private readonly ScoreViewModel _viewModel;

        public ScorePresenter(ScoreViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Dispose()
        {
        }
    }
}