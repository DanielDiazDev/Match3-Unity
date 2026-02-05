using System;
using Match3.Scripts.Utils;

namespace Match3.Scripts.Level
{
    public class LevelPresenter : IDisposable
    {
        private readonly LevelViewModel _viewModel;

        public LevelPresenter(LevelViewModel viewModel)
        {
            _viewModel = viewModel;
          //  EventDispatcherService.Instance.Subscribe<LevelModel>(OnLevelUpdated);
        }

        public void Present(LevelModel levelModel)
        {
            _viewModel.LevelId.Value = levelModel.id.ToString();
            _viewModel.Stars.Value = levelModel.stars;
            _viewModel.Unlocked.Value = levelModel.unlocked;
        }

        public void Dispose()
        {
           // EventDispatcherService.Instance.Unsubscribe<LevelModel>(OnLevelUpdated);
        }
    }
}