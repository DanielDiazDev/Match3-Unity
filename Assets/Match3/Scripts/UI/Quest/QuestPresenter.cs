using System;

namespace Match3.Scripts.Quest
{
    public class QuestPresenter : IDisposable
    {
        private readonly QuestViewModel _viewModel;

        public QuestPresenter(QuestViewModel viewModel)
        {
            _viewModel = viewModel;
        }

        public void Dispose()
        {
        }
    }
}